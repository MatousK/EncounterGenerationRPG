using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doors : MonoBehaviour
{
    public Grid MapGrid;
    public List<int> ConnectingRooms = new List<int>();
    public List<GameObject> OpenDoorsObjects = new List<GameObject>();
    public List<GameObject> ClosedDoorsObjects = new List<GameObject>();
    private PathfindingMapController pathfindingMapController;
    private RoomsLayout roomsLayout;
    private bool _IsOpened;
    public bool IsOpened
    {
        get
        {
            return _IsOpened;
        }
        set
        {
            _IsOpened = value;
            OnDoorOpenedChanged();
        }
    }
    void Awake()
    {
        roomsLayout = FindObjectOfType<RoomsLayout>();
        pathfindingMapController = FindObjectOfType<PathfindingMapController>();
        MapGrid = MapGrid != null ? MapGrid : FindObjectOfType<Grid>();
        GetComponent<InteractableObject>().OnInteractionTriggered += OnDoorsInteractedWith;
    }

    // Start is called before the first frame update
    void Start()
    {
        OnDoorOpenedChanged();
    }

    private void OnDoorsInteractedWith(object sender, Hero e)
    {
        IsOpened = true;
    }

    public void UpdatePathfindingMap(PathfindingMap map)
    {
        var activeDoorObjects = IsOpened ? OpenDoorsObjects : ClosedDoorsObjects;
        foreach (var doorObject in activeDoorObjects)
        {
            var coordinates = MapGrid.WorldToCell(doorObject.transform.position);
            map?.SetSquareIsPassable(coordinates.x, coordinates.y, IsOpened);
        }
    }

    void OnDoorOpenedChanged()
    {
        foreach (var openedDoor in OpenDoorsObjects)
        {
            openedDoor.SetActive(IsOpened);
        }
        foreach (var closedDoor in ClosedDoorsObjects)
        {
            closedDoor.SetActive(!IsOpened);
        }
        UpdatePathfindingMap(pathfindingMapController?.Map);
        if (IsOpened)
        {
            foreach (var roomIndex in ConnectingRooms)
            {
                roomsLayout.Rooms[roomIndex].IsExplored = true;
            }
        }
    }
}
