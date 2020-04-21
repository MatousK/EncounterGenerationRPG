using System.Collections.Generic;
using Assets.Scripts.Combat;
using UnityEngine;

namespace Assets.Scripts.Movement.Pathfinding
{
    class Pathfinder: MonoBehaviour
    {
        PathfindingMapController pathfindingMapController;
        Grid mapGrid;

        public List<Vector2Int> FindPath(Vector2 originWorldSpace, Vector2Int targetGridSpace, CombatantBase combatant, bool ignoreOtherCombatants = false)
        {
            EnsureDependenciesReady();
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
            var targetSquareLocal = mapData.GridCoordinatesToLocal(targetGridSpace);
            var originSquareLocal = mapData.GridCoordinatesToLocal(originSquare);
            var astarMap = mapData.PassableTilesMap;
            var path = (new AStar(astarMap, originSquareLocal, targetSquareLocal)).FindPath();
            return path != null ? LocalPathToGrid(path, mapData) : null;
        }

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