using System.Collections.Generic;
using System.Linq;
using Assets.ProceduralLevelGenerator.Scripts.GeneratorPipeline.Payloads.Interfaces;
using Assets.ProceduralLevelGenerator.Scripts.GeneratorPipeline.RoomTemplates.Doors;
using Assets.ProceduralLevelGenerator.Scripts.Pipeline;
using UnityEngine;
using Doors = Assets.Scripts.Environment.Doors;

namespace Assets.Scripts.DungeonGenerator
{
    class InitializeCorridorDoorsTask<TPayload> : ConfigurablePipelineTask<TPayload, InitializeCorridorDoorsConfig>
        where TPayload : class, IGeneratorPayload, IGraphBasedGeneratorPayload, INamedTilemapsPayload
    {
        public override void Process()
        {
            var roomsData = Payload.Layout.GetAllRoomInfo();
            foreach (var room in roomsData)
            {
                if (!room.IsCorridor)
                {
                    // We want to initialize configs only for doors in corridors.
                    continue;
                }
                var allGameDoors = Payload.GameObject.GetComponentsInChildren<Doors>();
                var roomGameDoors = allGameDoors.Where(doors => doors.GetComponent<RoomInfoComponent>().RoomIndex == room.GeneratorData.Node).ToArray();
                Debug.Assert(roomGameDoors.Length == 2 && room.Doors.Count == 2, "Corridors should have exactly two doors.");
                if (roomGameDoors.Length != 2 || room.Doors.Count != 2)
                {
                    continue;
                }
                InitializeDoors(roomGameDoors, room.Doors);
            }
        }

        void InitializeDoors(Doors[] gameDoors, List<DoorInfo<int>> generatorDoors)
        {
            // We assume that both doors are either horizontal or vertical.
            // TODO: Figure out how to match doors in some easier way.
            // We mean horizontal as in corridor goes from left to right. They mean whether the doors object itself should be vertical or horizontal, which is the opposite of what we want.
            var isHorizontal = !generatorDoors.First().IsHorizontal;
            var gameDoor1MainCoordinate = isHorizontal ? gameDoors[0].transform.position.x : gameDoors[0].transform.position.y;
            var gameDoor2MainCoordinate = isHorizontal ? gameDoors[1].transform.position.x : gameDoors[1].transform.position.y;
            // Start doors are the ones with lower X/Y values, the other is end doors. No real significance, just naming convention here.
            var startGameDoors = gameDoor1MainCoordinate < gameDoor2MainCoordinate ? gameDoors[0] : gameDoors[1];
            var endGameDoors = startGameDoors == gameDoors[0] ? gameDoors[1] : gameDoors[0];

            var generatorDoors1MainCoordinate = isHorizontal ? generatorDoors[0].DoorLine.From.x : generatorDoors[0].DoorLine.From.y;
            var generatorDoors2MainCoordinate = isHorizontal ? generatorDoors[1].DoorLine.From.x : generatorDoors[1].DoorLine.From.y;
            var startGeneratorDoors = generatorDoors1MainCoordinate < generatorDoors2MainCoordinate ? generatorDoors[0] : generatorDoors[1];
            var endGeneratorDoors = startGeneratorDoors == generatorDoors[0] ? generatorDoors[1] : generatorDoors[0];
            startGameDoors.ConnectingRooms.Add(startGeneratorDoors.ConnectedRoom);
            endGameDoors.ConnectingRooms.Add(endGeneratorDoors.ConnectedRoom);
        }
    }
}