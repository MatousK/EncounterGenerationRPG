using System.Collections.Generic;
using Assets.Scripts.GameFlow;
using UnityEngine;

namespace Assets.Scripts.DungeonGenerator
{
    /// <summary>
    /// Class managing the layout of the rooms.
    /// It is filled while generating the dungeon.
    /// While playing through the dungeon, this class is the single source of truth regarding the state of the doors.
    /// </summary>
    public class RoomsLayout : MonoBehaviour
    {
        /// <summary>
        /// The clas responsible for the flow of the game. We use it to detect when the level is reloaded.
        /// </summary>
        private GameStateManager gameStateManager;
        private void Update()
        {
            if (gameStateManager == null)
            {
                gameStateManager = FindObjectOfType<GameStateManager>();
                if (gameStateManager != null)
                {
                    gameStateManager.GameReloaded += GameStateManager_GameReloaded;
                }
            }
        }

        private void OnDestroy()
        {
            gameStateManager.GameReloaded -= GameStateManager_GameReloaded;
        }
        /// <summary>
        /// List of all rooms in the game.
        /// </summary>
        public List<RoomInfo> Rooms = new List<RoomInfo>();
        /// <summary>
        /// When the game is reloaded, this method resets the explored status of every room, making only the starting room unexplored.
        /// </summary>
        /// <param name="sender">Object which raised this event.</param>
        /// <param name="e">Arguments of the event.</param>
        private void GameStateManager_GameReloaded(object sender, System.EventArgs e)
        {
            foreach (var room in Rooms)
            {
                room.SetRoomExplored(room.IsStartingRoom);
            }
        }
    }
}
