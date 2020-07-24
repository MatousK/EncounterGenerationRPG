using System;
using System.Collections.Generic;
using System.Linq;
using Assets.ProceduralLevelGenerator.Scripts.GeneratorPipeline.Payloads.Interfaces;
using Assets.ProceduralLevelGenerator.Scripts.Pipeline;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.DungeonGenerator
{
    /// <summary>
    /// Adds components which give the other game components information about the room layout, which rooms are which etc. 
    /// Without this task the dungeon generator data would be unavailable to the game.
    /// Adds the <see cref="RoomsLayout"/> object to the map.
    /// Uses <see cref="RoomExporterConfig"/> for configuration.
    /// </summary>
    /// <typeparam name="TPayload"><inheritdoc/></typeparam>
    public class RoomExporterTask<TPayload> : ConfigurablePipelineTask<TPayload, RoomExporterConfig>
        where TPayload : class, IGeneratorPayload, IGraphBasedGeneratorPayload, INamedTilemapsPayload, IRoomToIntMappingPayload<RoomWithEncounter>
    {
        /// <summary>
        /// Adds the information about room layout to the generated map.
        /// </summary>
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
                roomInfo.IsBossFight = roomGraphData.IsBossRoom;
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
        /// <summary>
        /// Retrieve all the positions of all squares that are considered to be inside this room.
        /// </summary>
        /// <param name="room">Room whose squares will be returned.</param>
        /// <returns>All squares inside the room.</returns>
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
        /// <summary>
        /// Retrieve information about the room with the specified index.
        /// </summary>
        /// <param name="index">The index of the room to be returned.</param>
        /// <returns>The room with the specified index.</returns>
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