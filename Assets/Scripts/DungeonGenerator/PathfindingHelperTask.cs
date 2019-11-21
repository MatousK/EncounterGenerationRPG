using Assets.ProceduralLevelGenerator.Scripts.Data.Graphs;
using Assets.ProceduralLevelGenerator.Scripts.GeneratorPipeline.Payloads.Interfaces;
using Assets.ProceduralLevelGenerator.Scripts.GeneratorPipeline.RoomTemplates.Doors;
using Assets.ProceduralLevelGenerator.Scripts.Pipeline;
using Assets.ProceduralLevelGenerator.Scripts.Utils;
using MapGeneration.Interfaces.Core.MapLayouts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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