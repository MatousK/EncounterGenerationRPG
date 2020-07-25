using System.Collections.Generic;
using Assets.Scripts.Combat;
using UnityEngine;

namespace Assets.Scripts.Movement.Pathfinding
{
    /// <summary>
    /// A component that can find a path from some specified position to some other position.
    /// The algorithm used is simple A*, we don't really handle the fact that there is multiagent pathfinding.
    /// </summary>
    class Pathfinder: MonoBehaviour
    {
        /// <summary>
        /// The component which provides this component with the pathfinding map.
        /// </summary>
        PathfindingMapController pathfindingMapController;
        /// <summary>
        /// The grid on which the game is being played.
        /// </summary>
        Grid mapGrid;
        /// <summary>
        /// Finds the shortest path between this space and some target space.
        /// </summary>
        /// <param name="originWorldSpace">The square where we want to start the search.</param>
        /// <param name="targetGridSpace">The target square where we want to end up.</param>
        /// <param name="combatant">The combatant who will walk the resulting path.</param>
        /// <param name="ignoreOtherCombatants">If true, the combatant should be ignoring other combatants and treat them as passable.</param>
        /// <returns>The path to the target.</returns>
        public List<Vector2Int> FindPath(Vector2 originWorldSpace, Vector2Int targetGridSpace, CombatantBase combatant, bool ignoreOtherCombatants = false)
        {
            EnsureDependenciesReady();
            // First we have to find the map on which we will be moving.
            var mapData = ignoreOtherCombatants ? pathfindingMapController.Map : pathfindingMapController.GetPassabilityMapForCombatant(combatant);
            UnityEngine.Debug.Assert(mapData != null);
            if (mapData == null)
            {
                return null;
            }
            var originSquare3D = mapGrid.WorldToCell(originWorldSpace);
            var originSquare = new Vector2Int(originSquare3D.x, originSquare3D.y);
            if (!mapData.IsSquareInBounds(originSquare) || 
                !mapData.IsSquareInBounds(targetGridSpace))
            {
                return null;
            }
            // The pathfinding map uses a different coordinate system than the game grid, we must convert.
            var targetSquareLocal = mapData.GridCoordinatesToLocal(targetGridSpace);
            var originSquareLocal = mapData.GridCoordinatesToLocal(originSquare);
            var astarMap = mapData.PassableTilesMap;
            // Calculate the path and then convert it to the grid coordinates.
            var path = (new AStar(astarMap, originSquareLocal, targetSquareLocal)).FindPath();
            return path != null ? LocalPathToGrid(path, mapData) : null;
        }
        /// <summary>
        /// Converts the path found by A* to a path in grid space the agent can traverse.
        /// </summary>
        /// <param name="localPath">Path in local coordinates.</param>
        /// <param name="map">The pathfinding map used in the search.</param>
        /// <returns>The path in grid coordinates.</returns>
        private List<Vector2Int> LocalPathToGrid(List<Vector2Int> localPath, PathfindingMap map)
        {
            List<Vector2Int> toReturn = new List<Vector2Int>(localPath.Count);
            // Drop the first move, that is the origin no need to go there.
            for (int i = 1; i < localPath.Count; ++i)
            {
                var gridCoordinates = map.LocalCoordinatesToGrid(localPath[i]);
                toReturn.Add(gridCoordinates);
            }
            return toReturn;
        }
        /// <summary>
        /// Ensures that we have references to all dependencies.
        /// </summary>
        private void EnsureDependenciesReady()
        {
            if (mapGrid == null || pathfindingMapController == null)
            {
                pathfindingMapController = FindObjectOfType<PathfindingMapController>();
                mapGrid = mapGrid != null ? mapGrid : FindObjectOfType<Grid>();
            }
        }
    }
}