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
    private void Start()
    {
        pathfindingMapController = FindObjectOfType<PathfindingMapController>();
        MapGrid = MapGrid != null ? MapGrid : FindObjectOfType<Grid>();
    }

    public List<Vector2> FindPath(Vector2 originWorldSpace, Vector2 targetWorldSpace, CombatantBase combatant)
    {
        var mapData = pathfindingMapController.GetPassabilityMapForCombatant(combatant);
        Debug.Assert(mapData != null);
        if (mapData == null)
        {
            return null;
        }
        var originSquare3D = MapGrid.WorldToCell(originWorldSpace);
        var originSquare = new Vector2Int(originSquare3D.x, originSquare3D.y);
        var targetSquare3D = MapGrid.WorldToCell(targetWorldSpace);
        var targetSquare = new Vector2Int(targetSquare3D.x, targetSquare3D.y);
        if (!mapData.IsSquareInBounds(originSquare) || 
            !mapData.IsSquareInBounds(targetSquare))
        {
            return null;
        }
        var targetSquareLocal = mapData.GridCoordinatesToLocal(targetSquare);
        var originSquareLocal = mapData.GridCoordinatesToLocal(originSquare);
        var astarMap = mapData.PassableTilesMap;
        var path = (new AStar(astarMap, originSquareLocal, targetSquareLocal)).FindPath();
        return path != null ? LocalGridPathToWorldSpace(path, mapData) : null;
    }

    public List<Vector2> LocalGridPathToWorldSpace(List<Vector2Int> localPath, PathfindingMap map)
    {
        List<Vector2> toReturn = new List<Vector2>(localPath.Count);
        // Drop the first move, that is the origin no need to go there.
        for (int i = 1; i < localPath.Count; ++i)
        {
            var gridCoordinates = map.LocalCoordinatesToGrid(localPath[i]);
            var gridCoordinates3D = new Vector3Int(gridCoordinates.x, gridCoordinates.y, 0);
            var worldCoordinates = MapGrid.GetCellCenterWorld(gridCoordinates3D);
            toReturn.Add(worldCoordinates);
        }
        return toReturn;
    }
}