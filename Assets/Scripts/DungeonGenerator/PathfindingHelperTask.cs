using System.Collections.Generic;
using Assets.ProceduralLevelGenerator.Scripts.GeneratorPipeline.Payloads.Interfaces;
using Assets.ProceduralLevelGenerator.Scripts.Pipeline;
using Assets.Scripts.Movement.Pathfinding;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.DungeonGenerator
{
    /// <summary>
    /// Adds the pathfinding information to the map. Also adds the <see cref="PathfindingMapController"/> to the map. Uses <see cref="PathfindingConfig"/> for configuration.
    /// </summary>
    /// <typeparam name="TPayload"><inheritdoc/></typeparam>
    public class PathfindingHelperTask<TPayload> : ConfigurablePipelineTask<TPayload, PathfindingConfig>
        where TPayload : class, IGeneratorPayload, IGraphBasedGeneratorPayload, INamedTilemapsPayload
    {
        /// <summary>
        /// Stores on the map which tilemaps are walkable and which are not.
        /// </summary>
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
        /// <summary>
        /// If necessary, adds the specified tilemap to the list of blocking or non blocking tilemaps the pathfinding algorithm should consider.
        /// </summary>
        /// <param name="tilemap">The tilemap being processed right now.</param>
        /// <param name="tilemapNavigationType">Specifies how should the pathfinding algorithm approach this tilemap.</param>
        /// <param name="blockingTilemaps">List of blocking tilemaps. The method will add <paramref name="tilemap"/> to it if the tilemap is blocking.</param>
        /// <param name="walkableTilemaps">List of walkable tilemaps. The method will add <paramref name="tilemap"/> to it if the tilemap is walkable.</param>
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