using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Camera;
using Assets.Scripts.DungeonGenerator;
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
        private void Awake()
        {
            cameraCentering = FindObjectOfType<CameraCentering>();
            spawnManager = FindObjectOfType<CombatantSpawnManager>();
            roomsLayout = FindObjectOfType<RoomsLayout>();
        }

        private void Start()
        {
            var startingRoom = roomsLayout.Rooms.First(room => room.IsStartingRoom);
            foreach (var partyMember in InitialParty)
            {
                spawnManager.SpawnCombatant(partyMember, startingRoom);
            }
            cameraCentering.Center(startingRoom);
        }
    }
}
