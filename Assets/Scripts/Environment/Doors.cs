using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DoorOrientation
{
    Vertical,
    Horizontal
}
/// <summary>
/// Represents doors on the map that can be opened and closed. Will update pathfinding information when entered and will play the cutscene on doors open.
/// </summary>
public class Doors : MonoBehaviour
{
    /// <summary>
    /// If true, these doors should close automatically in combat.
    /// </summary>
    public bool CloseInCombat = true;
    /// <summary>
    /// Whether the doors connect rooms on a Y or X axis.
    /// </summary>
    public DoorOrientation Orientation;
    /// <summary>
    /// List of rooms these doors are connecting.
    /// </summary>
    public List<int> ConnectingRooms = new List<int>();
    /// <summary>
    /// All game objects which should be enabled iff these doors are opened.
    /// </summary>
    public List<GameObject> OpenDoorsObjects = new List<GameObject>();
    /// <summary>
    /// All game objects which should be enabled iff these doors are closed.
    /// </summary>
    public List<GameObject> ClosedDoorsObjects = new List<GameObject>();
    private Grid MapGrid;
    private PathfindingMapController pathfindingMapController;
    private RoomsLayout roomsLayout;
    private CutsceneManager cutsceneManager;
    private CombatantsManager combatantstManager;
    // If true, the player has already at least once opened these doors. Used to open these doores again after autoclosing for combat.
    private bool didPlayerOpenDoors;
    // The hero who opened this door if any.
    private Hero doorOpener;
    private bool _IsOpened;
    public bool IsOpened
    {
        get
        {
            return _IsOpened;
        }
        set
        {
            if (!IsOpened == value)
            {
                _IsOpened = value;
                OnDoorOpenedChanged();
            }
        }
    }
    void Awake()
    {
        combatantstManager = FindObjectOfType<CombatantsManager>();
        cutsceneManager = FindObjectOfType<CutsceneManager>();
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

    private void Update()
    {
        UpdateOpened();
    }

    private void UpdateOpened()
    {
        IsOpened = didPlayerOpenDoors && (!CloseInCombat || !combatantstManager.IsCombatActive);
    }

    private void OnDoorsInteractedWith(object sender, Hero e)
    {
        if (didPlayerOpenDoors)
        {
            // Never open doors more than once.
            return;
        }
        doorOpener = e;
        didPlayerOpenDoors = true;
        UpdateOpened();
        foreach (var roomIndex in ConnectingRooms)
        {
            if (!roomsLayout.Rooms[roomIndex].IsExplored)
            {
                roomsLayout.Rooms[roomIndex].ExploreRoom(this);
                var openDoorsCutscene = cutsceneManager.InstantiateCutscene<EnterRoomCutscene>();
                openDoorsCutscene.DoorOpener = doorOpener;
                openDoorsCutscene.OpenedDoors = this;
                cutsceneManager.PlayCutscene(openDoorsCutscene);
            }
        }
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
    }
}