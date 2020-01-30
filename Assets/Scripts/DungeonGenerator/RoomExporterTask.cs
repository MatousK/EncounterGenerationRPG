using Assets.ProceduralLevelGenerator.Scripts.GeneratorPipeline.Payloads.Interfaces;
using Assets.ProceduralLevelGenerator.Scripts.Pipeline;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Assets.ProceduralLevelGenerator.Scripts.Data.Graphs;
using Assets.ProceduralLevelGenerator.Scripts.GeneratorPipeline.Payloads;
using System;

public class RoomExporterTask<TPayload> : ConfigurablePipelineTask<TPayload, RoomExporterConfig>
    where TPayload : class, IGeneratorPayload, IGraphBasedGeneratorPayload, INamedTilemapsPayload, IRoomToIntMappingPayload<RoomWithEncounter>
{
    public override void Process()
    {
        var roomsLayout = Payload.GameObject.AddComponent<RoomsLayout>();
        var roomsData = Payload.Layout.GetAllRoomInfo();
        bool isFirstRoom = true;
        foreach (var room in roomsData)
        {
            RoomWithEncounter roomGraphData;
            if (!Payload.RoomToIntMapping.TryGetKey(room.GeneratorData.Node, out roomGraphData))
            {
                throw new ArgumentException("Room graph data");
            }
            var allTilemaps = room.Room.gameObject.GetComponentsInChildren<Tilemap>();
            HashSet<Vector2Int> roomSquares = new HashSet<Vector2Int>();
            foreach (var tilemap in allTilemaps)
            {
                foreach (var position in tilemap.cellBounds.allPositionsWithin)
                {
                    if (tilemap.GetTile(position) != null)
                    {
                        roomSquares.Add(new Vector2Int(position.x + room.Position.x, position.y + room.Position.y));
                    }
                }
            }
            var roomInfo = new RoomInfo();
            roomInfo.IsStartingRoom = isFirstRoom;
            if (isFirstRoom)
            {
                roomInfo.ExploreRoom();
            }
            roomInfo.RoomEncounter = roomGraphData.EncounterConfiguration;
            isFirstRoom = false;
            roomInfo.RoomSquaresPositions = roomSquares.ToList();
            roomsLayout.Rooms.Add(roomInfo);
        }
    }
}