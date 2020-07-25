using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Assets.Scripts.Combat;
using Assets.Scripts.Environment;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.Movement.Pathfinding
{
    /// <summary>
    /// The map which provides the correct <see cref="PathfindingMap"/> to other components.
    /// It is responsible for keeping the map updates.
    /// Our pathfinding algorithms are single agent, while we have multiple agents.
    /// To make sure all agents don't try to move to the same position, whenever a combatant tries to move somewhere,
    /// the taget position is considered reserved and other agents cannot move there
    /// </summary>
    [ExecuteAfter(typeof(TreasureChestManager))]
    public class PathfindingMapController: MonoBehaviour
    {
        /// <summary>
        /// All tilemaps the combatants can walk on.
        /// </summary>
        public List<Tilemap> WalkableTilemaps;
        /// <summary>
        /// All tilemaps which block movement.
        /// </summary>
        public List<Tilemap> CollisionTilemaps;
        /// <summary>
        /// The map containing pathfinding data.
        /// Can be modified from outside.
        /// Do only modifications which affect all navigation agents.
        /// </summary>
        public PathfindingMap Map;
        /// <summary>
        /// The component which knows about all combatants in the game.
        /// </summary>
        CombatantsManager combatantsManager;
        /// <summary>
        /// Called before the first Update method. Initializes the pathfinding map as much as possible.
        /// </summary>
        void Start()
        {
            combatantsManager = FindObjectOfType<CombatantsManager>();
            var bounds = CalculateMapBounds();
            Map = new PathfindingMap(bounds);
            FillPassableTiles();
            FillBlockingTiles();
            // HACK: Doors's Start method is being called before this, so the pathfinding map is not updated.
            // Probably doors are ready sooner... For whatever reason, we have to manually update the pathfinding map based on the doors.
            var doors = FindObjectsOfType<Doors>();
            foreach (var door in doors)
            {
                door.UpdatePathfindingMap(Map);
            }
            // And the same thing fo treasure chests.
            // TODO: Give tags to all objects which can update navigation and give them an interface with UpdatePathfindingMap.
            var treasureChestManager = FindObjectOfType<TreasureChestManager>();
            if (treasureChestManager != null)
            {
                treasureChestManager.UpdatePathfindingMap();
            }
        }
        /// <summary>
        /// Retrieve the with all squares considered passable for a given combatant.
        /// Combatants can reserve their target position. Other combatants will then consider those target places to be impassable.
        /// </summary>
        /// <param name="navigatingCombatant">The combatant navigating through the map.</param>
        /// <returns>A pathfinding map specific to the given combatant.</returns>
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
        /// <summary>
        /// Get the closest passable square for the <paramref name="navigatingCombatant"/> within <paramref name="maxDistance"/> of the <paramref name="targetPostion"/>.
        /// </summary>
        /// <param name="navigatingCombatant">The combatant who will be navigating.</param>
        /// <param name="targetPostion">The target position of the movement.</param>
        /// <param name="maxDistance">Maximum distance from the target allowed to move there.</param>
        /// <returns></returns>
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
        /// <summary>
        /// Goes through all passable tilemaps and sets those spaces as passable.
        /// </summary>
        void FillPassableTiles()
        {
            foreach (Tilemap tileMap in WalkableTilemaps)
            {
                FillPathfindingMapForTilemap(tileMap, true);
            }
        }
        /// <summary>
        /// Goes through all blocking tilemaps and sets those spaces as impassable.
        /// </summary>
        void FillBlockingTiles()
        {
            foreach (Tilemap tileMap in CollisionTilemaps)
            {
                FillPathfindingMapForTilemap(tileMap, false);
            }
        }
        /// <summary>
        /// Go through the <paramref name="tilemap"/>, setting all squares defined on it as impassable or passable, based on the <paramref name="isPassable"/> parameter.
        /// </summary>
        /// <param name="tilemap">The relevant tilemap.</param>
        /// <param name="isPassable">Whether the squares on the <paramref name="tilemap"/> are blocking or not.</param>
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
        /// <summary>
        /// Calculates the bounds of the map.
        /// We only consider walkable tilemaps, as the player cannot go outside those bounds.
        /// </summary>
        /// <returns>Bounds of the map.</returns>
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