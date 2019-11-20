using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

class PathfindingMapController: MonoBehaviour
{
    public List<Tilemap> WalkableTilemaps;
    public List<Tilemap> CollisionTilemaps;
    public PathfindingMap Map;
    Grid mapGrid;
    CombatantsManager combatantsManager;
    void Start()
    {
        var bounds = CalculateMapBounds();
        Map = new PathfindingMap(bounds);
        mapGrid = FindObjectOfType<Grid>();
        combatantsManager = FindObjectOfType<CombatantsManager>();
        FillPassableTiles();
        FillBlockingTiles();
        print("Success");
    }

    public PathfindingMap GetPassabilityMapForCombatant(CombatantBase navigatingCombatant)
    {
        var toReturn = Map.Clone();
        foreach (var combatant in combatantsManager.GetAllCombatants())
        {
            if (combatant == navigatingCombatant)
            {
                continue;
            }
            Vector2 combatantWorldSpacePosition = combatant.transform.position;
            var combatantGridPosition = mapGrid.WorldToCell(combatantWorldSpacePosition);
            toReturn.SetSquareIsPassable(combatantGridPosition.x, combatantGridPosition.y, false);
        }
        return toReturn;
    }

    void FillPassableTiles()
    {
        foreach (Tilemap tileMap in WalkableTilemaps)
        {
            FillPathfindingMapForTilemap(tileMap, true);
        }
    }

    void FillBlockingTiles()
    {
        foreach (Tilemap tileMap in CollisionTilemaps)
        {
            FillPathfindingMapForTilemap(tileMap, false);
        }
    }

    void FillPathfindingMapForTilemap(Tilemap tilemap, bool isPassable)
    {
        Vector3Int currentPosition = new Vector3Int();
        for (int x = tilemap.cellBounds.xMin; x <= tilemap.cellBounds.xMax; ++x)
        {
            for (int y = tilemap.cellBounds.yMin; y <= tilemap.cellBounds.yMax; ++y)
            {
                currentPosition.x = x;
                currentPosition.y = y;
                if (tilemap.GetTile(currentPosition))
                {
                    Map.SetSquareIsPassable(x, y, isPassable);
                }
            }
        }
    }

    BoundsInt CalculateMapBounds()
    {
        var xMin = int.MaxValue;
        var yMin = int.MaxValue;
        var xMax = int.MinValue;
        var yMax = int.MinValue;
        var allTileMaps = WalkableTilemaps.Concat(CollisionTilemaps);
        foreach (var tilemap in allTileMaps)
        {
            var bounds = tilemap.cellBounds;
            xMin = bounds.xMin < xMin ? bounds.xMin : xMin;
            yMin = bounds.yMin < yMin ? bounds.yMin : yMin;
            xMax = bounds.xMax > xMax ? bounds.xMax : xMax;
            yMax = bounds.yMax > yMax ? bounds.yMax : yMax;
        }
        return new BoundsInt(xMin, yMin, 0, xMax - xMin, yMax - yMin, 0);
    }
}