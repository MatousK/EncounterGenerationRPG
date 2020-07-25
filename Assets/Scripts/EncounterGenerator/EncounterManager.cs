using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Analytics;
using Assets.Scripts.Combat;
using Assets.Scripts.DungeonGenerator;
using Assets.Scripts.EncounterGenerator.Algorithm;
using Assets.Scripts.EncounterGenerator.Configuration;
using Assets.Scripts.EncounterGenerator.Development;
using Assets.Scripts.EncounterGenerator.Model;
using Assets.Scripts.Environment;
using Assets.Scripts.Experiment;
using Assets.Scripts.GameFlow;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts.EncounterGenerator
{
    /// <summary>
    /// The component on the game scene which generates the proper encounters when appropriate.
    /// </summary>
    class EncounterManager : MonoBehaviour
    {
        /// <summary>
        /// The object which can update the matrix when a combat is over.
        /// </summary>
        public EncounterMatrixUpdater MatrixUpdater { get; private set; }
        /// <summary>
        /// The object which does the actual encounter generation.
        /// </summary>
        private EncounterGenerator encounterGenerator;
        /// <summary>
        /// Used to access information about all the rooms in the dungeon.
        /// </summary>
        private RoomsLayout roomsLayout;
        /// <summary>
        /// The object which can spawn new combatants in the game.
        /// </summary>
        private CombatantSpawnManager combatantSpawnManager;
        /// <summary>
        /// Knows about all combatants in the game.
        /// </summary>
        private CombatantsManager combatantsManager;
        /// <summary>
        /// Notifies other components about game over and reloaded events.. 
        /// </summary>
        private GameStateManager gameStateManager;
        /// <summary>
        /// The general configuration for the encounter generator.
        /// </summary>
        private readonly EncounterGeneratorConfiguration generatorConfiguration = EncounterGeneratorConfiguration.CurrentConfig;
        /// <summary>
        /// The manager which holds the difficulty matrix that should be used by the encounter generator.
        /// </summary>
        private DifficultyMatrixProvider difficultyMatrixProvider;
        /// <summary>
        /// The object which knows about all levels. Most importantly for this class, it knows which encounters should be generated and which should be static.
        /// </summary>
        private LevelLoader levelLoader;
        /// <summary>
        /// Called before the first update, finds all dependencies on the scene, attaches to relevant events and creates the <see cref="encounterGenerator"/> and <see cref="MatrixUpdater"/>.
        /// </summary>
        private void Start()
        {
            var analyticsService = FindObjectsOfType<AnalyticsService>().FirstOrDefault(analytics => !analytics.IsPendingKill);
            difficultyMatrixProvider =
                FindObjectsOfType<DifficultyMatrixProvider>().First(provider => !provider.IsPendingKill);
            UnityEngine.Debug.Log($"Encounter manager found matrix: {difficultyMatrixProvider}");
            levelLoader = FindObjectsOfType<LevelLoader>().FirstOrDefault(loader => !loader.IsPendingKill);
            var difficultyMatrix = difficultyMatrixProvider.CurrentDifficultyMatrix;
            MatrixUpdater = new EncounterMatrixUpdater(difficultyMatrix, generatorConfiguration, analyticsService);
            MatrixUpdater.MatrixChanged += MatrixUpdater_MatrixChanged;
            encounterGenerator = new EncounterGenerator(difficultyMatrix, MatrixUpdater, generatorConfiguration);
            roomsLayout = FindObjectOfType<RoomsLayout>();
            combatantSpawnManager = FindObjectOfType<CombatantSpawnManager>();
            combatantsManager = FindObjectOfType<CombatantsManager>();
            gameStateManager = FindObjectOfType<GameStateManager>();
            combatantsManager.CombatOver += CombatantsManager_CombatOver;
            gameStateManager.GameOver += GameStateManager_GameOver;
            foreach (var room in roomsLayout.Rooms)
            {
                room.IsExploredChanged += OnRoomExplored;
            }
        }
        /// <summary>
        /// Called when the matrix is changed after combat. Propagates the event to the difficulty matrix updater.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Arguments of the event, i.e. what caused the change of the matrix.</param>
        private void MatrixUpdater_MatrixChanged(object sender, MatrixChangedEventArgs e)
        {
            if (difficultyMatrixProvider != null)
            {
                difficultyMatrixProvider.OnMatrixChanged(e);
            }
        }
        /// <summary>
        /// When the game is over, this method logs the results of the combat and says that the party was defeated.
        /// </summary>
        /// <param name="sender">The sender of this event.</param>
        /// <param name="e">Arguments of the event.</param>
        private void GameStateManager_GameOver(object sender, EventArgs e)
        {
            LogCombatResult(true);
        }
        /// <summary>
        /// When a combat is over, this method logs the result of the combat and says that the party won.
        /// </summary>
        /// <param name="sender">The sender of this event.</param>
        /// <param name="e">Arguments of the event.</param>
        private void CombatantsManager_CombatOver(object sender, EventArgs e)
        {
            LogCombatResult(false);
        }
        /// <summary>
        /// Called when a room is explored, this method spawns the appropriate monsters in the room.
        /// This method is responsible for both static and dynamic encounters.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="exploredEventArgs">Information about which room was explored.</param>
        private void OnRoomExplored(object sender, RoomExploredEventArgs exploredEventArgs)
        {
            var exploredRoom = (RoomInfo)sender;
            if (!exploredRoom.IsExplored)
            {
                // Room got changed to not explored, does not really interest us.
                return;
            }

            combatantsManager.IsBossFight = exploredRoom.IsBossFight;
            var allHeroes = combatantsManager.PlayerCharacters;
            var partyDefinition = new PartyDefinition { PartyMembers = allHeroes };
            List<GameObject> encounter;
            switch (levelLoader.CurrentEncounterGenerationAlgorithm)
            {
                case EncounterGenerationAlgorithmType.MatrixBasedGenerator:
                    MatrixUpdater.AdjustMatrixForNextFight = true;
                    MatrixUpdater.IsStaticEncounter = false;
                    encounter = encounterGenerator.GenerateEncounters(exploredRoom.RoomEncounter, partyDefinition);
                    break;
                case EncounterGenerationAlgorithmType.StaticGenerator:
                    MatrixUpdater.IsStaticEncounter = true;
                    MatrixUpdater.AdjustMatrixForNextFight = levelLoader.AdjustMatrixForStaticEncounters;
                    encounter = exploredRoom.StaticMonsters;
                    var staticEncounter =
                        EncounterDefinition.GetDefinitionFromMonsters(exploredRoom.StaticMonsters);
                    UnityEngine.Debug.Log($"Calculating difficulty. Matrix provider: {difficultyMatrixProvider}");
                    UnityEngine.Debug.Log($"Calculating difficulty. Matrix : {difficultyMatrixProvider.CurrentDifficultyMatrix}");
                    var encounterDifficulty =
                        difficultyMatrixProvider.CurrentDifficultyMatrix.GetDifficultyFor(staticEncounter,
                            partyDefinition, generatorConfiguration);
                    MatrixUpdater.StoreCombatStartConditions(partyDefinition, staticEncounter, encounterDifficulty);
                    UnityEngine.Debug.Log($"Expected difficulty for this static encounter is {encounterDifficulty}");
                    break;
                default:
                    throw new Exception("Unknown monster generation algorithm");
            }
            SpawnMonsters(encounter, sender as RoomInfo, exploredEventArgs.IncomingDoors);
        }
        /// <summary>
        /// Spawns the <paramref name="monstersToSpawn"/> in a <paramref name="room"/>.
        /// </summary>
        /// <param name="monstersToSpawn">Monsters that should be spawned in the room.</param>
        /// <param name="room">Room in which the monsters should be spawned.</param>
        /// <param name="incomingDoors">Doors through which the party entered. The monsters will not be spawned too close to those doors if possible.</param>
        private void SpawnMonsters(List<GameObject> monstersToSpawn, RoomInfo room, Doors incomingDoors)
        {
            combatantSpawnManager.SpawnCombatants(monstersToSpawn, room, incomingDoors, 5);
        }
        /// <summary>
        /// Called when a combat is over, this method instructs the matrix to log the results of the combat.
        /// </summary>
        /// <param name="wasGameOver">If true, the party was defeated in this combat.</param>
        private void LogCombatResult(bool wasGameOver)
        {
            var allHeroes = combatantsManager.PlayerCharacters;
            var partyDefinition = new PartyDefinition { PartyMembers = allHeroes };
            MatrixUpdater.CombatOverAdjustMatrix(partyDefinition, wasGameOver);
        }
        /// <summary>
        /// When destroyed, unsubscribe from all events.
        /// </summary>
        private void OnDestroy()
        {
            if (roomsLayout != null)
            {
                foreach (var room in roomsLayout.Rooms)
                {
                    room.IsExploredChanged -= OnRoomExplored;
                }
            }

            if (combatantsManager != null)
            {
                combatantsManager.CombatOver -= CombatantsManager_CombatOver;
            }

            if (gameStateManager != null)
            {
                gameStateManager.GameOver -= GameStateManager_GameOver;
            }

            if (MatrixUpdater != null)
            {
                MatrixUpdater.MatrixChanged -= MatrixUpdater_MatrixChanged;
            }
        }

    }
}