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
    /// </summary>
    [ExecuteAfter(typeof(PathfindingMapController)), ExecuteAfter(typeof(Doors))]
    public class InitialPartyManager: MonoBehaviour
    {
        public List<GameObject> InitialParty;

        private CameraCentering cameraCentering;
        private CombatantSpawnManager spawnManager;
        private RoomsLayout roomsLayout;
        private GameStateManager gameStateManager;
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

        private void GameStateManager_GameReloaded(object sender, System.EventArgs e)
        {
            combatantsManager.DestroyPlayerCharacters();
            SpawnPartyAndRecenterCamera();
        }
    }
}
