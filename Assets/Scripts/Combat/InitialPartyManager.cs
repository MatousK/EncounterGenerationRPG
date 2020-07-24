using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Camera;
using Assets.Scripts.CombatSimulator;
using Assets.Scripts.DungeonGenerator;
using Assets.Scripts.Environment;
using Assets.Scripts.GameFlow;
using Assets.Scripts.Movement.Pathfinding;
using UnityEngine;

namespace Assets.Scripts.Combat
{
    /// <summary>
    /// Responsible for spawning the party when the game starts.
    /// Will spawn the party once initialized. Will also respawn the party on restart.
    /// </summary>
    [ExecuteAfter(typeof(PathfindingMapController)), ExecuteAfter(typeof(Doors))]
    public class InitialPartyManager: MonoBehaviour
    {
        /// <summary>
        /// Templates for the party that should be spawned.
        /// </summary>
        public List<GameObject> InitialParty;
        /// <summary>
        /// Class that can center the camera on the room where the party spawns.
        /// </summary>
        private CameraCentering cameraCentering;
        /// <summary>
        /// Class which can actually spawn combatants onto a map.
        /// </summary>
        private CombatantSpawnManager spawnManager;
        /// <summary>
        /// Contains dungeon generator information, used to determine in which room the party should spawn and the room bounds.
        /// </summary>
        private RoomsLayout roomsLayout;
        /// <summary>
        /// Object which notifies the scene about game over and game reset. Used to respawn the party on reset.
        /// </summary>
        private GameStateManager gameStateManager;
        /// <summary>
        /// Object which should know about all combatants in the game.
        /// </summary>
        private CombatantsManager combatantsManager;

        private void Start()
        {
            cameraCentering = FindObjectOfType<CameraCentering>();
            spawnManager = FindObjectOfType<CombatantSpawnManager>();
            roomsLayout = FindObjectOfType<RoomsLayout>();
            gameStateManager = FindObjectOfType<GameStateManager>();
            gameStateManager.GameReloaded += GameStateManager_GameReloaded;
            combatantsManager = FindObjectOfType<CombatantsManager>();
            SpawnPartyAndRecenterCamera();
        }
        /// <summary>
        /// Find the start room of the floor and spawn the party there.
        /// If party member's attributes are set in <see cref="LevelLoader.CurrentPartyConfiguration"/>, sets attributes of the party mebmerrs.
        /// </summary>
        private void SpawnPartyAndRecenterCamera()
        {
            var startingRoom = roomsLayout.Rooms.First(room => room.IsStartingRoom);
            var levelLoader = FindObjectsOfType<LevelLoader>().FirstOrDefault(loader => !loader.IsPendingKill);
            var partyConfiguration = levelLoader != null ? levelLoader.CurrentPartyConfiguration : null;
            var spawnedCombatants = spawnManager.SpawnCombatants(InitialParty, startingRoom);
            foreach (var spawnedCombatant in spawnedCombatants)
            {
                var newHero = spawnedCombatant.GetComponent<Hero>();
                var partyMemberConfig = partyConfiguration?.GetStatsFor(newHero.HeroProfession);
                if (partyMemberConfig != null)
                {
                    newHero.Attributes.DealtDamageMultiplier = partyMemberConfig.Value.AttackModifier;
                    newHero.SetTotalMaxHp(partyMemberConfig.Value.MaxHp);
                }

            }
            cameraCentering.Center(startingRoom);
        }
        /// <summary>
        /// On game reload, we should kill the old characters and recreate the new one.
        /// </summary>
        /// <param name="sender">The sender of this event.</param>
        /// <param name="e">The arguments of the event.</param>
        private void GameStateManager_GameReloaded(object sender, System.EventArgs e)
        {
            combatantsManager.DestroyPlayerCharacters();
            SpawnPartyAndRecenterCamera();
        }
    }
}
