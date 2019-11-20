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

    public List<Vector2> FindPath(Vector2 originWorldSpace, Vector2 targetWorldSpace)
    {
        var mapData = pathfindingMapController.Map;
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
            !mapData.IsSquareInBounds(targetSquare) ||
            !mapData.GetSquareIsPassable(targetSquare) ||
            !mapData.GetSquareIsPassable(originSquare))
        {
            return null;
        }
        var targetSquareLocal = mapData.GridCoordinatesToLocal(targetSquare);
        var originSquareLocal = mapData.GridCoordinatesToLocal(originSquare);
        return null;
    }
}