using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.EncounterGenerator.Configuration;
using Assets.Scripts.Environment;
using UnityEngine;

namespace Assets.Scripts.DungeonGenerator
{
    public struct RoomExploredEventArgs
    {
        public RoomExploredEventArgs(Doors incomingDoors)
        {
            IncomingDoors = incomingDoors;
        }
        public Doors IncomingDoors;
    }
    [Serializable]
    public class RoomInfo
    {
        public EncounterConfiguration RoomEncounter;
        public int TreasureChestsMax;
        public int TreasureChestsMin;
        public List<Vector2Int> RoomSquaresPositions;
        public List<Vector2Int> ConnectedCorridorsSquares = new List<Vector2Int>();
        public bool IsStartingRoom;
        public event EventHandler<RoomExploredEventArgs> IsExploredChanged;
        [SerializeField]
        private bool isExplored;
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