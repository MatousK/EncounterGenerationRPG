using System;
using System.Collections.Generic;
using System.Linq;
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
    class EncounterManager : MonoBehaviour
    {
        public EncounterMatrixUpdater MatrixUpdater { get; private set; }
        // TODO: Debug, remove in release builds.
        private StaticEncounterGenerator staticEncounterGenerator;
        private EncounterGenerator encounterGenerator;
        private RoomsLayout roomsLayout;
        private CombatantSpawnManager combatantSpawnManager;
        private CombatantsManager combatantsManager;
        private GameStateManager gameStateManager;
        private readonly EncounterGeneratorConfiguration generatorConfiguration = new EncounterGeneratorConfiguration();
        private DifficultyMatrixProvider difficultyMatrixProvider;
        private LevelLoader levelLoader;

        private void Start()
        {
            difficultyMatrixProvider =
                FindObjectsOfType<DifficultyMatrixProvider>().First(provider => !provider.IsPendingKill);
            levelLoader = FindObjectsOfType<LevelLoader>().First(loader => !loader.IsPendingKill);
            var difficultyMatrix = difficultyMatrixProvider.CurrentDifficultyMatrix;
            MatrixUpdater = new EncounterMatrixUpdater(difficultyMatrix, generatorConfiguration);
            MatrixUpdater.MatrixChanged += MatrixUpdater_MatrixChanged;
            encounterGenerator = new EncounterGenerator(difficultyMatrix, MatrixUpdater, generatorConfiguration);
            staticEncounterGenerator = GetComponent<StaticEncounterGenerator>();
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

        private void MatrixUpdater_MatrixChanged(object sender, MatrixChangedEventArgs e)
        {
            if (difficultyMatrixProvider != null)
            {
                difficultyMatrixProvider.OnMatrixChanged(e);
            }
        }

        private void GameStateManager_GameOver(object sender, EventArgs e)
        {
            LogCombatResult(true);
        }

        private void CombatantsManager_CombatOver(object sender, EventArgs e)
        {
            LogCombatResult(false);
        }

        private void OnRoomExplored(object sender, RoomExploredEventArgs exploredEventArgs)
        {
            var exploredRoom = (RoomInfo)sender;
            if (!exploredRoom.IsExplored)
            {
                // Room got changed to not explored, does not really interest us.
                return;
            }
            var allHeroes = combatantsManager.PlayerCharacters;
            var partyDefinition = new PartyDefinition { PartyMembers = allHeroes };
            List<GameObject> encounter;
            switch (levelLoader.CurrentEncounterGenerationAlgorithm)
            {
                case EncounterGenerationAlgorithmType.MatrixBasedGenerator:
                    encounter = encounterGenerator.GenerateEncounters(exploredRoom.RoomEncounter, partyDefinition);
                    break;
                case EncounterGenerationAlgorithmType.StaticGenerator:
                    encounter = exploredRoom.StaticMonsters;
                    if (levelLoader.AdjustMatrixForStaticEncounters)
                    {
                        var staticEncounter =
                            EncounterDefinition.GetDefinitionFromMonsters(exploredRoom.StaticMonsters);
                        var encounterDifficulty =
                            difficultyMatrixProvider.CurrentDifficultyMatrix.GetDifficultyFor(staticEncounter,
                                partyDefinition, generatorConfiguration);
                        MatrixUpdater.StoreCombatStartConditions(partyDefinition, staticEncounter, encounterDifficulty);
                        UnityEngine.Debug.Log($"Expected difficulty for this static encounter is {encounterDifficulty}");
                    }
                    break;
                default:
                    throw new Exception("Unknown monster generation algorithm");
            }
            SpawnMonsters(encounter, sender as RoomInfo, exploredEventArgs.IncomingDoors);
        }

        private void SpawnMonsters(List<GameObject> monstersToSpawn, RoomInfo room, Doors incomingDoors)
        {
            combatantSpawnManager.SpawnCombatants(monstersToSpawn, room, incomingDoors, 5);
        }

        private void LogCombatResult(bool wasGameOver)
        {
            var allHeroes = combatantsManager.PlayerCharacters;
            var partyDefinition = new PartyDefinition { PartyMembers = allHeroes };
            MatrixUpdater.CombatOverAdjustMatrix(partyDefinition, wasGameOver);
        }

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