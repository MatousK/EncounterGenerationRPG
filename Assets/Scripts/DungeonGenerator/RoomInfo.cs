using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.EncounterGenerator.Configuration;
using Assets.Scripts.Environment;
using UnityEngine;

namespace Assets.Scripts.DungeonGenerator
{
    /// <summary>
    /// Event arguments for the event which happens when some doors are opened.
    /// </summary>
    public struct RoomExploredEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoomExploredEventArgs"/> class.
        /// </summary>
        /// <param name="incomingDoors">The doors whose opening caused this room to be explored.</param>
        public RoomExploredEventArgs(Doors incomingDoors)
        {
            IncomingDoors = incomingDoors;
        }
        /// <summary>
        /// The doors which the player opened which caused this room to be explored.
        /// </summary>
        public Doors IncomingDoors;
    }
    /// <summary>
    /// Data about a room available during the game.
    /// </summary>
    [Serializable]
    public class RoomInfo
    {
        /// <summary>
        /// If true, this room contains a boss fight,
        /// </summary>
        public bool IsBossFight;
        /// <summary>
        /// The list of monsters which should appear in this room if we are currently experimenting with static encounters.
        /// </summary>
        public List<GameObject> StaticMonsters;
        /// <summary>
        /// The configuration for the encounter generator specifying which encounter should happen in this room.
        /// </summary>
        public EncounterConfiguration RoomEncounter;
        /// <summary>
        /// How many chests with permanent health power ups should appear in this room.
        /// </summary>
        public int HealthBonusTreasureChests;
        /// <summary>
        /// How many chests with permanent damage power ups should appear in this room.
        /// </summary>
        public int DamageBonusTreasureChests;
        /// <summary>
        /// How many chests with healing potions should appear in this room.
        /// </summary>
        public int HealingPotionsTreasureChests;
        /// <summary>
        /// Index of this room. This is basically a room ID.
        /// </summary>
        public int Index;
        /// <summary>
        /// Positions of all squares inside this room.
        /// </summary>
        public List<Vector2Int> RoomSquaresPositions;
        /// <summary>
        /// Positions of all squares in corridors connected to this room.
        /// </summary>
        public List<Vector2Int> ConnectedCorridorsSquares = new List<Vector2Int>();
        /// <summary>
        /// If true, the players should start in this room.
        /// </summary>
        public bool IsStartingRoom;
        /// <summary>
        /// This event is raised when the room is explored.
        /// </summary>
        public event EventHandler<RoomExploredEventArgs> IsExploredChanged;
        /// <summary>
        /// If true, this room is explored.
        /// </summary>
        [SerializeField]
        private bool isExplored;
        /// <summary>
        /// If true, this room is explored. No getter to force the exploration to happen through SetRoomExplored.
        /// </summary>
        public bool IsExplored => isExplored;
        /// <summary>
        /// Sets this room as explored, optionally setting from which doors the room was explored.
        /// </summary>
        /// <param name="incomingDoors">The doors whose opening explored this room.</param>
        public void SetRoomExplored(bool isExplored, Doors incomingDoors = null)
        {
            this.isExplored = isExplored;
            IsExploredChanged?.Invoke(this, new RoomExploredEventArgs(incomingDoors));
        }
        /// <summary>
        /// Retrieve the bounds containing this room. Not containing the connecting corridors.
        /// </summary>
        /// <param name="onGrid">Grid representing the game world. The bounds should be relative to this grid.</param>
        /// <returns>Bounds of this room in the game world.</returns>
        public Bounds GetBounds(Grid onGrid)
        {
            Bounds toReturn = new Bounds(onGrid.CellToWorld(new Vector3Int(RoomSquaresPositions.First().x, RoomSquaresPositions.First().y, 0)), Vector3.one);
            foreach (var square in RoomSquaresPositions)
            {
                toReturn.Encapsulate(onGrid.CellToWorld(new Vector3Int(square.x, square.y, 0)));
            }

            return toReturn;
        }
    }
}