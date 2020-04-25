using System;
using System.Collections.Generic;
using System.Linq;
using Assets.ProceduralLevelGenerator.Scripts.GeneratorPipeline.Payloads.Interfaces;
using Assets.ProceduralLevelGenerator.Scripts.Pipeline;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.DungeonGenerator
{
    public class RoomExporterTask<TPayload> : ConfigurablePipelineTask<TPayload, RoomExporterConfig>
        where TPayload : class, IGeneratorPayload, IGraphBasedGeneratorPayload, INamedTilemapsPayload, IRoomToIntMappingPayload<RoomWithEncounter>
    {
        public override void Process()
        {
            var roomsLayout = Payload.GameObject.AddComponent<RoomsLayout>();
            var roomsData = Payload.Layout.GetAllRoomInfo();
            bool isFirstRoom = true;
            foreach (var room in roomsData.Where(room => !room.IsCorridor))
            {
                var roomInfo = new RoomInfo
                {
                    IsStartingRoom = isFirstRoom
                };
                if (isFirstRoom)
                {
                    roomInfo.SetRoomExplored(true);
                }
                var roomGraphData = GetRoomData(room.GeneratorData.Node);
                roomInfo.RoomEncounter = roomGraphData.EncounterConfiguration;
                roomInfo.StaticMonsters = roomGraphData.StaticMonsters;
                roomInfo.Index = room.GeneratorData.Node;
                roomInfo.HealingPotionsTreasureChests = roomGraphData.HealingPotionChests;
                roomInfo.DamageBonusTreasureChests = roomGraphData.DamageBonusChests;
                roomInfo.HealthBonusTreasureChests = roomGraphData.HealthBonusChests;
                isFirstRoom = false;
                roomInfo.RoomSquaresPositions = GetRoomSquares(room).ToList();
                roomsLayout.Rooms.Add(roomInfo);
            }

            foreach (var corridor in roomsData.Where(room => room.IsCorridor))
            {
                foreach (var connectingRoom in corridor.Doors.Select(doors => doors.ConnectedRoom))
                {
                    var roomInfo = roomsLayout.Rooms[connectingRoom];
                    roomInfo.ConnectedCorridorsSquares.AddRange(GetRoomSquares(corridor));
                }
            }
        }

        HashSet<Vector2Int> GetRoomSquares(Assets.ProceduralLevelGenerator.Scripts.GeneratorPipeline.RoomTemplates.RoomInfo<int> room)
        {
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
            return roomSquares;
        }

        RoomWithEncounter GetRoomData(int index)
        {
            if (!Payload.RoomToIntMapping.TryGetKey(index, out RoomWithEncounter roomGraphData))
            {
                throw new ArgumentException("Room graph data invalid");
            }
            return roomGraphData;
        }
    }
}