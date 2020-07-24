using System.Collections;
using System.Collections.Generic;
using Assets.ProceduralLevelGenerator.Scripts.Pipeline;
using UnityEngine;

namespace Assets.Scripts.DungeonGenerator
{
    /// <summary>
    /// Configuration for the task which adds pathfinding information to the generated level, see <see cref="PathfindingHelperTask{TPayload}"/>.
    /// This complex architecture is required by the library to allow the tasks to be generic.
    /// </summary>
    [CreateAssetMenu(menuName = "Dungeon generator/Encounter Generation RPG/Pathfinding task", fileName = "PathfindingTask")]
    public class PathfindingConfig : PipelineConfig
    {
        /// <summary>
        /// Specif how the tiles on the floor tilemap affect navigation.
        /// </summary>
        public LayerNavigationType FloorNavigation = LayerNavigationType.Walkable;
        /// <summary>
        /// Specif how the tiles on the walls tilemap affect navigation.
        /// </summary>
        public LayerNavigationType WallsNavigation = LayerNavigationType.Blocking;
        /// <summary>
        /// Specif how the tiles on the collider tilemap affect navigation.
        /// </summary>
        public LayerNavigationType ColliderNavigation = LayerNavigationType.Blocking;
    }
    /// <summary>
    /// Assigned to a tilemap layer, this tells the pathfinding logic how should it treat this tilemap.
    /// </summary>
    public enum LayerNavigationType
    {
        /// <summary>
        /// This tilemap can be walked on.
        /// </summary>
        Walkable,
        /// <summary>
        /// This tilemap blocks movement.
        /// </summary>
        Blocking,
        /// <summary>
        /// This tilemap is irrelevant in regards to pathfinding.
        /// </summary>
        DoesNotAffectNavigation
    }
}