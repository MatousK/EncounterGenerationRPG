using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class PathfindingMap
{
    public PathfindingMap(BoundsInt tilemapBounds)
    {
        this.tilemapBounds = tilemapBounds;
        PassableTilesMap = new bool[tilemapBounds.size.x, tilemapBounds.size.y];
    }

    BoundsInt tilemapBounds;
    /// <summary>
    /// Map of all tiles and if they are passable or not.
    /// Is in local coordinates, do not use without translating using GridCoordinatesToLocal pr LocalCoordinatesToGrid
    /// </summary>
    public bool[,] PassableTilesMap;

    public void SetSquareIsPassable(int x, int y, bool isPassable)
    {
        var coordinates = GridCoordinatesToLocal(x, y);
        PassableTilesMap[coordinates.x, coordinates.y] = isPassable;
    }

    public bool GetSquareIsPassable(Vector2Int squareCoordinates)
    {
        return GetSquareIsPassable(squareCoordinates.x, squareCoordinates.y);
    }

    public bool IsSquareInBounds(Vector2Int squarePosition)
    {
        return IsSquareInBounds(squarePosition.x, squarePosition.y);
    }

    public bool IsSquareInBounds(int x, int y)
    {
        return x >= tilemapBounds.xMin && 
               x <= tilemapBounds.xMax &&
               y >= tilemapBounds.yMin &&
               y <= tilemapBounds.yMax;
    }

    public bool GetSquareIsPassable(int x, int y)
    {
        var coordinates = GridCoordinatesToLocal(x, y);
        return PassableTilesMap[coordinates.x, coordinates.y];
    }

    public Vector2Int LocalCoordinatesToGrid(Vector2Int localCoordinates)
    {
        return LocalCoordinatesToGrid(localCoordinates.x, localCoordinates.y);
    }

    public Vector2Int LocalCoordinatesToGrid(int x, int y)
    {
        return new Vector2Int(x + tilemapBounds.xMin, y + tilemapBounds.yMin);
    }

    public Vector2Int GridCoordinatesToLocal(Vector2Int gridCoordinates)
    {
        return GridCoordinatesToLocal(gridCoordinates.x, gridCoordinates.y);
    }

    public Vector2Int GridCoordinatesToLocal(int x, int y)
    {
        return new Vector2Int(x - tilemapBounds.xMin, y - tilemapBounds.yMin);
    }
}
