using UnityEngine;

namespace Assets.Scripts.DungeonGenerator
{
    /// <summary>
    /// Component for game objects which need to know in which room do they exist.
    /// </summary>
    public class RoomInfoComponent: MonoBehaviour
    {
        /// <summary>
        /// Index of room where this game object can be found.
        /// </summary>
        public int RoomIndex;
    }
}