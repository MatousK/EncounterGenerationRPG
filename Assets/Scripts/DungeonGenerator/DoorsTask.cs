using Assets.ProceduralLevelGenerator.Scripts.Data.Graphs;
using Assets.ProceduralLevelGenerator.Scripts.GeneratorPipeline.Payloads.Interfaces;
using Assets.ProceduralLevelGenerator.Scripts.GeneratorPipeline.RoomTemplates.Doors;
using Assets.ProceduralLevelGenerator.Scripts.Pipeline;
using Assets.ProceduralLevelGenerator.Scripts.Utils;
using MapGeneration.Interfaces.Core.MapLayouts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorsTask<TPayload> : ConfigurablePipelineTask<TPayload, DoorsConfig>
    where TPayload : class, IGeneratorPayload, IGraphBasedGeneratorPayload, INamedTilemapsPayload
{
    Grid grid;
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
            }
        }
    }

    protected void RemoveWalls(OrthogonalLine doorLine)
    {
        foreach (var point in doorLine.GetPoints())
        {
            Payload.WallsTilemap.SetTile(point, null);
        }
    }

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

    protected Vector2 GetLocalCoordinatesForDoorLine(OrthogonalLine doorLine)
    {
        var doorlineStartLocal = grid.GetCellCenterLocal(doorLine.From);
        var doorlineEndLocal = grid.GetCellCenterLocal(doorLine.To);
        return (doorlineStartLocal + doorlineEndLocal) / 2;
    }
}