using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class Pathfinder: MonoBehaviour
{
    PathfindingMapController pathfindingMapController;
    public Grid MapGrid;
    private void Awake()
    {
        pathfindingMapController = FindObjectOfType<PathfindingMapController>();
        MapGrid = MapGrid != null ? MapGrid : FindObjectOfType<Grid>();
    }

    public List<Vector2Int> FindPath(Vector2 originWorldSpace, Vector2Int targetGridSpace, CombatantBase combatant)
    {
        var mapData = pathfindingMapController.GetPassabilityMapForCombatant(combatant);
        Debug.Assert(mapData != null);
        if (mapData == null)
        {
            return null;
        }
        var originSquare3D = MapGrid.WorldToCell(originWorldSpace);
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
}