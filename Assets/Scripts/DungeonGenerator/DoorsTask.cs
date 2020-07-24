using System.Collections.Generic;
using Assets.ProceduralLevelGenerator.Scripts.GeneratorPipeline.Payloads.Interfaces;
using Assets.ProceduralLevelGenerator.Scripts.Pipeline;
using Assets.ProceduralLevelGenerator.Scripts.Utils;
using Assets.Scripts.Environment;
using UnityEngine;

namespace Assets.Scripts.DungeonGenerator
{
    /// <summary>
    /// Adds the doors to the map. Uses <see cref="DoorsConfig"/> for configuration.
    /// </summary>
    /// <typeparam name="TPayload"><inheritdoc/></typeparam>
    public class DoorsTask<TPayload> : ConfigurablePipelineTask<TPayload, DoorsConfig>
        where TPayload : class, IGeneratorPayload, IGraphBasedGeneratorPayload, INamedTilemapsPayload
    {
        /// <summary>
        /// Grid onto which the doors should be placed.
        /// </summary>
        Grid grid;
        /// <summary>
        /// Adds the doors to the map.
        /// </summary>
        public override void Process()
        {
            if (!Config.AddDoors)
            {
                return;
            }
            grid = Payload.GameObject.GetComponentInChildren<Grid>();
            HashSet<Vector2> doorsGeneratedAtPositions = new HashSet<Vector2>();
            var roomsData = Payload.Layout.GetAllRoomInfo();
            foreach (var room in roomsData)
            {
                foreach (var doors in room.Doors)
                {
                    var doorLine = doors.DoorLine;
                    RemoveWalls(doorLine);
                    var doorLocalPosition = GetLocalCoordinatesForDoorLine(doorLine);
                    if (doorsGeneratedAtPositions.Contains(doorLocalPosition))
                    {
                        // Doors definition exist in both rooms they connect, yet they should be generated only once.
                        continue;
                    }
                    doorsGeneratedAtPositions.Add(doorLocalPosition);
                    var doorTemplate = GetDoorTemplateObject(doors.IsHorizontal, doors.FacingDirection);
                    var newDoor = Object.Instantiate(doorTemplate, Payload.GameObject.transform);
                    newDoor.transform.localPosition = doorLocalPosition;
                    var doorsComponent = newDoor.GetComponent<Doors>();
                    doorsComponent.ConnectingRooms.Add(room.GeneratorData.Node);
                    doorsComponent.ConnectingRooms.Add(doors.ConnectedRoom);
                }
            }
        }
        /// <summary>
        /// Remove the walls to make space for the doors. Removes only from the wall tilemap.
        /// </summary>
        /// <param name="doorLine">Defines the place where the doors will be spawned.</param>
        protected void RemoveWalls(OrthogonalLine doorLine)
        {
            foreach (var point in doorLine.GetPoints())
            {
                Payload.WallsTilemap.SetTile(point, null);
            }
        }
        /// <summary>
        /// Retrieve the doors game object that should be spawned.
        /// </summary>
        /// <param name="isHorizontal">If true, these doors are horizontal, i.e. the character will go through them from left to right or vice versa.</param>
        /// <param name="facingDirection">Orientation of the doors.</param>
        /// <returns>Template of the doors that should be added to the map.</returns>
        protected GameObject GetDoorTemplateObject(bool isHorizontal, Vector2Int facingDirection)
        {
            if (isHorizontal)
            {
                if (facingDirection.y > 0)
                {
                    return Config.VerticalDoorsTop;
                }
                else
                {
                    return Config.VerticalDoorsBottom;
                }
            }
            else
            {
                if (facingDirection.x > 0)
                {
                    return Config.HorizontalDoorsRight;
                }
                else
                {
                    return Config.HorizontalDoorsLeft;
                }
            }
        }
        /// <summary>
        /// Retrieve the position where the doors should be spawned for some defined door line.
        /// </summary>
        /// <param name="doorLine">Where the algorithm says the doors should be.</param>
        /// <returns>The grid coordinates where the doors should be spawned.</returns>
        protected Vector2 GetLocalCoordinatesForDoorLine(OrthogonalLine doorLine)
        {
            var doorlineStartLocal = grid.GetCellCenterLocal(doorLine.From);
            var doorlineEndLocal = grid.GetCellCenterLocal(doorLine.To);
            return (doorlineStartLocal + doorlineEndLocal) / 2;
        }
    }
}