using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doors : MonoBehaviour
{
    public float DistanceToOpen = 1;
    public List<GameObject> OpenDoorsObjects = new List<GameObject>();
    public List<GameObject> ClosedDoorsObjects = new List<GameObject>();
    private CombatantsManager combatantsManager;
    private PathfindingMapController pathfindingMapController;
    public Grid MapGrid;
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
        pathfindingMapController = FindObjectOfType<PathfindingMapController>();
        combatantsManager = FindObjectOfType<CombatantsManager>();
        MapGrid = MapGrid != null ? MapGrid : FindObjectOfType<Grid>();
    }
    // Start is called before the first frame update
    void Start()
    {
        OnDoorOpenedChanged();
    }

    void Update()
    {
        if (IsOpened)
        {
            // Doors can only close, once opened, do nothing.
            return;
        }
        if (combatantsManager.IsCombatActive)
        {
            // Doors can only open when not in combat.
            return;
        }
        foreach (var playerCharacter in combatantsManager.PlayerCharacters)
        {
            var distanceFromDoors = Vector2.Distance(playerCharacter.transform.position, transform.position);
            if (distanceFromDoors < DistanceToOpen)
            {
                IsOpened = true;
                break;
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
