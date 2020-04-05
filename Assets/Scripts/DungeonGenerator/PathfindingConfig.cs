using System.Collections;
using System.Collections.Generic;
using Assets.ProceduralLevelGenerator.Scripts.Pipeline;
using UnityEngine;

namespace Assets.Scripts.DungeonGenerator
{
    [CreateAssetMenu(menuName = "Dungeon generator/Encounter Generation RPG/Pathfinding task", fileName = "PathfindingTask")]
    public class PathfindingConfig : PipelineConfig
    {
        public LayerNavigationType FloorNavigation = LayerNavigationType.Walkable;
        public LayerNavigationType WallsNavigation = LayerNavigationType.Blocking;
        public LayerNavigationType ColliderNavigation = LayerNavigationType.Blocking;
    }

    public enum LayerNavigationType
    {
        Walkable,
        Blocking,
        DoesNotAffectNavigation
    }
}