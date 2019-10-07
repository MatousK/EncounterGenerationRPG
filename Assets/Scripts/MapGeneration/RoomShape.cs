using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using GeneralAlgorithms.DataStructures.Common;

[Serializable]
public struct DoorShape
{
    public Vector2Int Start;
    public Vector2Int End;
    public OrthogonalLine ToOrthogonalLine()
    {
        return new OrthogonalLine(Start.ToMapGeneratorVector(), End.ToMapGeneratorVector());
    }
}

public class RoomShape : MonoBehaviour
{
    // The shape of the room, one tile is one unit
    public List<Vector2Int> ShapePolygon;
    // Minimum distance from a corner to the door.
    public int DoorCornerDistance = 1;
    // How many tiles to the doors take.
    public int DoorSize = 2;
    // If set to positive values, doors are forced to spawn at this position.
    public List<DoorShape> ForcedDoorPositions;
    public GameObject CollisionTileMapObject;
}
