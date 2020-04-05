using System.Collections.Generic;
using Assets.ProceduralLevelGenerator.Scripts.GeneratorPipeline.Payloads.Interfaces;
using Assets.ProceduralLevelGenerator.Scripts.Pipeline;
using Assets.Scripts.Movement.Pathfinding;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.DungeonGenerator
{
    public class PathfindingHelperTask<TPayload> : ConfigurablePipelineTask<TPayload, PathfindingConfig>
        where TPayload : class, IGeneratorPayload, IGraphBasedGeneratorPayload, INamedTilemapsPayload
    {
        public override void Process()
        {
            List<Tilemap> blockingTilemaps = new List<Tilemap>();
            List<Tilemap> walkableTilemaps = new List<Tilemap>();
            HandleTilemap(Payload.FloorTilemap, Config.FloorNavigation, blockingTilemaps, walkableTilemaps);
            HandleTilemap(Payload.CollideableTilemap, Config.ColliderNavigation, blockingTilemaps, walkableTilemaps);
            HandleTilemap(Payload.WallsTilemap, Config.WallsNavigation, blockingTilemaps, walkableTilemaps);
            PathfindingMapController controllerComponent = Payload.GameObject.AddComponent<PathfindingMapController>();
            controllerComponent.CollisionTilemaps = blockingTilemaps;
            controllerComponent.WalkableTilemaps = walkableTilemaps;
        }

        void HandleTilemap(Tilemap tilemap, LayerNavigationType tilemapNavigationType, List<Tilemap> blockingTilemaps, List<Tilemap> walkableTilemaps)
        {
            switch (tilemapNavigationType)
            {
                case LayerNavigationType.Blocking:
                    blockingTilemaps.Add(tilemap);
                    break;
                case LayerNavigationType.Walkable:
                    walkableTilemaps.Add(tilemap);
                    break;
                case LayerNavigationType.DoesNotAffectNavigation:
                    break;
            }
        }
    }
}