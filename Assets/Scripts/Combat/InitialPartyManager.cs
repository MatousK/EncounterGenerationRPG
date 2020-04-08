using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Camera;
using Assets.Scripts.DungeonGenerator;
using Assets.Scripts.GameFlow;
using UnityEngine;

namespace Assets.Scripts.Combat
{
    /// <summary>
    /// Responsible for spawning the party when the game starts.
    /// </summary>
    public class InitialPartyManager: MonoBehaviour
    {
        public List<GameObject> InitialParty;

        private CameraCentering cameraCentering;
        private CombatantSpawnManager spawnManager;
        private RoomsLayout roomsLayout;
        private GameStateManager gameStateManager;
        private CombatantsManager combatantsManager;
        private void Awake()
        {
            cameraCentering = FindObjectOfType<CameraCentering>();
            spawnManager = FindObjectOfType<CombatantSpawnManager>();
            roomsLayout = FindObjectOfType<RoomsLayout>();
            gameStateManager = FindObjectOfType<GameStateManager>();
            gameStateManager.GameReloaded += GameStateManager_GameReloaded;
            combatantsManager = FindObjectOfType<CombatantsManager>();
        }

        private void Start()
        {
            SpawnPartyAndRecenterCamera();
        }

        private void SpawnPartyAndRecenterCamera()
        {
            var startingRoom = roomsLayout.Rooms.First(room => room.IsStartingRoom);
            foreach (var partyMember in InitialParty)
            {
                spawnManager.SpawnCombatant(partyMember, startingRoom);
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
