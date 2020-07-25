using System.Linq;
using Assets.Scripts.DungeonGenerator;
using Assets.Scripts.Movement.Pathfinding;
using UnityEngine;

namespace Assets.Scripts.Environment
{
    /// <summary>
    /// This manager goes through the room layout information from the dungeon generator and makes sure each room contains the chests with correct drop types.
    /// The room templates contain more treasure chests than needed, this class disables those chests that are unused.
    /// </summary>
    class TreasureChestManager : MonoBehaviour
    {
        /// <summary>
        /// If true, this object already did its initialization logic.
        /// Hack to ensure that this class will eventually mark the chests as impassable, either when it is initialized or when the pathfinding map is initialized.
        /// </summary>
        private bool didInitialize;
        /// <summary>
        /// Called before the first frame, sets the drop rate for all chests and disable the unused ones.
        /// </summary>
        void Start()
        {
            // In every room there are some treasures placed by the designer.
            // As we already know how many treasures should be in each room, we can now disable the chests that we won't end up using.
            var random = new System.Random();

            var roomLayout = FindObjectOfType<RoomsLayout>();
            var allTreasureChests = FindObjectsOfType<TreasureChest>();
            // Group the treasure chests by rooms and then process the chests spawned in each of these rooms.
            var perRoomTreasureChests = allTreasureChests.GroupBy(chest => chest.GetComponent<RoomInfoComponent>().RoomIndex);
            foreach (var chestGroup in perRoomTreasureChests)
            {
                var chestRoom = roomLayout.Rooms[chestGroup.Key];
                var roomTreasureChests = chestGroup.ToArray();
                var treasuresToKeepCount = chestRoom.HealthBonusTreasureChests + chestRoom.DamageBonusTreasureChests + chestRoom.HealingPotionsTreasureChests;
                // We can only have as many treasure chests in the room as were specified.
                treasuresToKeepCount = treasuresToKeepCount < roomTreasureChests.Length ? treasuresToKeepCount : roomTreasureChests.Length;
                // Disable the treasures which we do not want to keep.
                for (int i = treasuresToKeepCount; i < roomTreasureChests.Length; ++i)
                {
                    roomTreasureChests[i].gameObject.SetActive(false);
                }
                // Set drops for chests. we want to keep
                for (int i = 0; i < treasuresToKeepCount; ++i)
                {
                    if (i < chestRoom.HealthBonusTreasureChests)
                    {
                        roomTreasureChests[i].TreasureToDrop = TreasureChestDrop.HealthBonus;
                    }
                    else if (i < chestRoom.HealthBonusTreasureChests + chestRoom.DamageBonusTreasureChests)
                    {
                        roomTreasureChests[i].TreasureToDrop = TreasureChestDrop.DamageBonus;
                    }
                    else
                    {
                        roomTreasureChests[i].TreasureToDrop = TreasureChestDrop.HealingPotion;
                    }
                }
            }

            didInitialize = true;
            UpdatePathfindingMap();
        }
        /// <summary>
        /// Updates the pathfinding map. Called automatically after <see cref="Start"/> finishes, but can be called also from the <see cref="PathfindingMapController"/> if it is initialized after this object.
        /// Marks the squares with chests as impassable.
        /// </summary>
        public void UpdatePathfindingMap()
        {
            var pathfindingMapController = FindObjectOfType<PathfindingMapController>();
            if (!didInitialize || pathfindingMapController == null || pathfindingMapController.Map == null)
            {
                return;
            }

            var map = pathfindingMapController.Map;
            var grid = FindObjectOfType<Grid>();
            foreach (var chest in FindObjectsOfType<TreasureChest>())
            {
                var coordinates = grid.WorldToCell(new Vector3(chest.transform.position.x,
                    chest.transform.position.y,
                    chest.transform.position.z));
                map.SetSquareIsPassable(coordinates.x, coordinates.y, false);
            }
        }
    }
}