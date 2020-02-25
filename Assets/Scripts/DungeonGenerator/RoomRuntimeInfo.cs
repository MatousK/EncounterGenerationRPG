using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public struct RoomExploredEventArgs
{
    public RoomExploredEventArgs(Doors incominingDoors)
    {
        IncomingDoors = incominingDoors;
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
    private bool _IsExplored;
    public bool IsExplored {
        get
        {
            return _IsExplored;
        }
    }
    /// <summary>
    /// Sets this room as explored, optionally setting from which doors the room was explored.
    /// </summary>
    /// <param name="incomingDoors">The doors whose opening explored this room.</param>
    public void ExploreRoom(Doors incomingDoors = null)
    {
        _IsExplored = true;
        IsExploredChanged?.Invoke(this, new RoomExploredEventArgs(incomingDoors));
    }
}
