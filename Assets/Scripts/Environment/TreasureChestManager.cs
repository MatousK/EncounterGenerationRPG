using System.Linq;
using Assets.Scripts.DungeonGenerator;
using Assets.Scripts.Movement.Pathfinding;
using UnityEngine;

namespace Assets.Scripts.Environment
{
    class TreasureChestManager : MonoBehaviour
    {
        private bool didInitialize;

        void Start()
        {
            // In every room there are some treasures placed by the designer.
            // As we already know how many treasures should be in each room, we can now disable the chests that we won't end up using.
            var random = new System.Random();

            var roomLayout = FindObjectOfType<RoomsLayout>();
            var allTreasureChests = FindObjectsOfType<TreasureChest>();
            // We go through the chests one by one, depending on room in which they belong.
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
                // Set drops for chests.
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