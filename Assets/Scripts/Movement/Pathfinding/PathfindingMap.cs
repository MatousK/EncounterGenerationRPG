using UnityEngine;

namespace Assets.Scripts.Movement.Pathfinding
{
    /// <summary>
    /// A class containing information about the current pathfinding map - which squares are possible and which are not.
    /// Can also do translation from grid space coordinates to coordinates used in pathfinding.
    /// </summary>
    public class PathfindingMap
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PathfindingMap"/> class.
        /// </summary>
        /// <param name="tilemapBounds">The bounds around all the squares on the map.</param>
        public PathfindingMap(BoundsInt tilemapBounds)
        {
            this.tilemapBounds = tilemapBounds;
            PassableTilesMap = new bool[tilemapBounds.size.x, tilemapBounds.size.y];
        }
        /// <summary>
        /// The bounds around all the squares on the map.
        /// </summary>
        BoundsInt tilemapBounds;
        /// <summary>
        /// Map of all tiles and if they are passable or not.
        /// Is in local coordinates, do not use without translating using GridCoordinatesToLocal pr LocalCoordinatesToGrid
        /// </summary>
        public bool[,] PassableTilesMap;
        /// <summary>
        /// Creates a duplicate of this map.
        /// </summary>
        /// <returns></returns>
        public PathfindingMap Clone()
        {
            var toReturn = (PathfindingMap)MemberwiseClone();
            toReturn.PassableTilesMap = (bool[,])PassableTilesMap.Clone();
            toReturn.tilemapBounds = tilemapBounds;
            return toReturn;
        }
        /// <summary>
        /// Sets that a specific square in grid coordinates is passable.
        /// </summary>
        /// <param name="x">The X coordinate of the square.</param>
        /// <param name="y">The Y coordinate of the square.</param>
        /// <param name="isPassable">True if the square should be passable, otherwise false.</param>
        public void SetSquareIsPassable(int x, int y, bool isPassable)
        {
            var coordinates = GridCoordinatesToLocal(x, y);
            PassableTilesMap[coordinates.x, coordinates.y] = isPassable;
        }
        /// <summary>
        /// Gets whether a specific square is passable in grid coordinates.
        /// </summary>
        /// <param name="squareCoordinates">The grid coordinates of the square.</param>
        /// <returns>True if the square is passable, false if impassable.</returns>
        public bool GetSquareIsPassable(Vector2Int squareCoordinates)
        {
            return GetSquareIsPassable(squareCoordinates.x, squareCoordinates.y);
        }
        /// <summary>
        /// Checks if the specified square is within the grid space bounds.
        /// </summary>
        /// <param name="squarePosition">The position for which we want to know if it is in bounds.</param>
        /// <returns>True if the space is in bounds, false if it is out of bounds.</returns>
        public bool IsSquareInBounds(Vector2Int squarePosition)
        {
            return IsSquareInBounds(squarePosition.x, squarePosition.y);
        }
        /// <summary>
        /// Checks if the specified square is within the grid space bounds.
        /// </summary>
        /// <param name="x">The X coordinate of the square.</param>
        /// <param name="y">The Y coordinate of the square.</param>
        /// <returns>True if the space is in bounds, false if it is out of bounds.</returns>
        public bool IsSquareInBounds(int x, int y)
        {
            return x >= tilemapBounds.xMin && 
                   x <= tilemapBounds.xMax &&
                   y >= tilemapBounds.yMin &&
                   y <= tilemapBounds.yMax;
        }
        /// <summary>
        /// Gets whether a specific square is passable in grid coordinates.
        /// </summary>
        /// <param name="x">The X coordinate of the square.</param>
        /// <param name="y">The Y coordinate of the square.</param>
        /// <returns>True if the square is passable, false if impassable.</returns>
        public bool GetSquareIsPassable(int x, int y)
        {
            var coordinates = GridCoordinatesToLocal(x, y);
            if (coordinates.x < 0 || 
                coordinates.y < 0 || 
                coordinates.x >= PassableTilesMap.GetLength(0) || 
                coordinates.y >= PassableTilesMap.GetLength(1))
            {
                return false;
            }
            return PassableTilesMap[coordinates.x, coordinates.y];
        }
        /// <summary>
        /// Converts <paramref name="localCoordinates"/> to the grid coordinates.
        /// </summary>
        /// <param name="localCoordinates">The coordinates to convert.</param>
        /// <returns>The grid coordinates of the <paramref name="localCoordinates"/> square.</returns>
        public Vector2Int LocalCoordinatesToGrid(Vector2Int localCoordinates)
        {
            return LocalCoordinatesToGrid(localCoordinates.x, localCoordinates.y);
        }
        /// <summary>
        /// Converts the specified local coordinates to grid coordinates.
        /// </summary>
        /// <param name="x">The X coordinate of the square.</param>
        /// <param name="y">The Y coordinate of the square.</param>
        /// <returns>The grid coordinates of the square.</returns>
        public Vector2Int LocalCoordinatesToGrid(int x, int y)
        {
            return new Vector2Int(x + tilemapBounds.xMin, y + tilemapBounds.yMin);
        }
        /// <summary>
        /// Converts the coordinates of <paramref name="gridCoordinates"/> to the local space coordinates.
        /// </summary>
        /// <param name="gridCoordinates">The coordinates to convert.</param>
        /// <returns>Local space coordinates,</returns>
        public Vector2Int GridCoordinatesToLocal(Vector2Int gridCoordinates)
        {
            return GridCoordinatesToLocal(gridCoordinates.x, gridCoordinates.y);
        }
        /// <summary>
        /// Converts the coordinates of the specified square to the local space coordinates.
        /// </summary>
        /// <param name="x">The X coordinate of the square.</param>
        /// <param name="y">The Y coordinate of the square.</param>
        /// <returns>Local space coordinates,</returns>
        public Vector2Int GridCoordinatesToLocal(int x, int y)
        {
            return new Vector2Int(x - tilemapBounds.xMin, y - tilemapBounds.yMin);
        }
    }
}
