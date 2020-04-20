using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Combat;
using Assets.Scripts.Environment;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.Movement.Pathfinding
{
    public class PathfindingMapController: MonoBehaviour
    {
        public List<Tilemap> WalkableTilemaps;
        public List<Tilemap> CollisionTilemaps;
        public PathfindingMap Map;
        CombatantsManager combatantsManager;
        void Start()
        { 
        }
        // We cannot use Start or Awake methods, because this must be called in a very precise moment of initialization. Sucks, I know.
        public void Init()
        {
            // This entire thing is a hack to ensure that we can load the pathfinding map after 
            combatantsManager = FindObjectOfType<CombatantsManager>();
            var bounds = CalculateMapBounds();
            Map = new PathfindingMap(bounds);
            FillPassableTiles();
            FillBlockingTiles();
            var doors = FindObjectsOfType<Doors>();
            foreach (var door in doors)
            {
                door.UpdatePathfindingMap(Map);
            }
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
                var combatantGridPosition = combatant.GetComponent<MovementController>().GetReservedGridPosition();
                if (combatantGridPosition != null)
                {
                    toReturn.SetSquareIsPassable(combatantGridPosition.Value.x, combatantGridPosition.Value.y, false);
                }
            }
            return toReturn;
        }

        public Vector2Int? GetPassableSpaceInDistance(CombatantBase navigatingCombatant, Vector2Int targetPostion, int maxDistance)
        {
            var passabilityMap = GetPassabilityMapForCombatant(navigatingCombatant);

            Vector2Int? closestPassableSquare = null;
            float closestPassableSquareDistance = float.MaxValue;
            for (int x = targetPostion.x - maxDistance; x <= targetPostion.x + maxDistance; ++x)
            { 
                for (int y = targetPostion.y - maxDistance; y <= targetPostion.y + maxDistance; ++y)
                {
                    var distanceFromTarget = Vector2Int.Distance(targetPostion, new Vector2Int(x, y));
                    if (passabilityMap.GetSquareIsPassable(x, y) && distanceFromTarget < closestPassableSquareDistance)
                    {
                        closestPassableSquareDistance = distanceFromTarget;
                        closestPassableSquare = new Vector2Int(x, y);
                    }
                }
            }
            return closestPassableSquare;
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
}