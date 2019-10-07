using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MapGeneration.Core.MapDescriptions;
using MapGeneration.Utils;
using MapGeneration.Core.Doors;
using GeneralAlgorithms.DataStructures.Polygons;
using GeneralAlgorithms.DataStructures.Common;
using MapGeneration.Interfaces.Core.Doors;
using System;
using MapGeneration.Core.Doors.DoorModes;
using MapGeneration.Interfaces.Core.MapDescriptions;

[Serializable]
public struct LockedRoomShape
{
    public int Position;
    public GameObject RoomShape;
}
public class MapController : MonoBehaviour
{
    public List<GameObject> AllowedRandomShapes;
    public List<LockedRoomShape> LockedRoomShapes;
    public int RoomCount;
    Dictionary<IRoomDescription, GameObject> RoomsGeneratorToUnityRepresentation = new Dictionary<IRoomDescription, GameObject>();
    Grid MapGrid;
    // Start is called before the first frame update
    void Start() {
        MapGrid = GetComponent<Grid>();
        GenerateLayout();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void GenerateLayout()
    {
        var generator = LayoutGeneratorFactory.GetDefaultChainBasedGenerator<int>();
        var mapDescription = new MapDescription<int>();
        for (int i = 0; i < RoomCount; ++i)
        {
            mapDescription.AddRoom(i);
            if (i != 0)
            {
                mapDescription.AddPassage(i - 1, i);
            }
        }
        foreach (var allowedRoomShape in AllowedRandomShapes)
        {
            var roomDescription = GetRoomShapeDescription(allowedRoomShape);
            Debug.Assert(roomDescription != null);
            if (roomDescription != null)
            {
                mapDescription.AddRoomShapes(roomDescription);
            }
        }
        mapDescription.AddRoomShapes(AllowedRandomShapes.ConvertAll(roomShape => GetRoomShapeDescription(roomShape)));
        foreach (var forcedRoomShape in LockedRoomShapes)
        {
            var roomDescription = GetRoomShapeDescription(forcedRoomShape.RoomShape);
            Debug.Assert(roomDescription != null);
            if (roomDescription != null)
            {
                mapDescription.AddRoomShapes(forcedRoomShape.Position, roomDescription);
            }
        }
        var layouts = generator.GetLayouts(mapDescription, 1);
        foreach (var layout in layouts)
        {
            foreach (var room in layout.Rooms)
            {
                var prefabToCreate = RoomsGeneratorToUnityRepresentation[room.RoomDescription];
                var newRoom = Instantiate(prefabToCreate);
                newRoom.transform.SetParent(MapGrid.transform);
                var shapeComponent = newRoom.GetComponent<RoomShape>();
                newRoom.transform.localPosition = new Vector3(room.Position.X, room.Position.Y, 1);
                newRoom.SetActive(true);
            }
        }
    }

    private RoomDescription GetRoomShapeDescription(GameObject RoomShapeObject)
    {
        var roomShape = RoomShapeObject.GetComponent<RoomShape>();
        if (roomShape == null)
        {
            Debug.Assert(false, "Received invalid object as roomshape");
            return null;
        }
        var vertices = roomShape.ShapePolygon.ConvertAll(vector => vector.ToMapGeneratorVector());
        var roomPolygon = new GridPolygon(vertices);
        IDoorMode usedDoorMode;
        if (roomShape == null || roomShape.ForcedDoorPositions.Count == 0)
        {
            usedDoorMode = new OverlapMode(roomShape.DoorSize, roomShape.DoorCornerDistance);
        }
        else
        {
            usedDoorMode = new SpecificPositionsMode(roomShape.ForcedDoorPositions.ConvertAll(doorPosition => doorPosition.ToOrthogonalLine()));
        }
        var toReturn = new RoomDescription(roomPolygon, usedDoorMode);
        RoomsGeneratorToUnityRepresentation[toReturn] = RoomShapeObject;
        return toReturn;
    }
}
