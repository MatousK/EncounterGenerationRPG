using System.Collections.Generic;
using Assets.Scripts.GameFlow;
using UnityEngine;

namespace Assets.Scripts.DungeonGenerator
{
    public class RoomsLayout : MonoBehaviour
    {
        private GameStateManager gameStateManager;
        private void Awake()
        {
            gameStateManager = FindObjectOfType<GameStateManager>();
            gameStateManager.GameReloaded += GameStateManager_GameReloaded;
        }

        public List<RoomInfo> Rooms = new List<RoomInfo>();

        private void GameStateManager_GameReloaded(object sender, System.EventArgs e)
        {
            foreach (var room in Rooms)
            {
                room.SetRoomExplored(room.IsStartingRoom);
            }
        }
    }
}
