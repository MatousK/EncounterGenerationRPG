using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class DoorDifficultyIndicator : MonoBehaviour
{
    public List<DifficultyColorPair> DifficultyIndicators;
    Tuple<RoomInfo, RoomInfo> ConnectedRooms;
    Doors Doors;
    EncounterManager EncounterManager;

    private void Start()
    {
        Doors = GetComponent<Doors>();
        var allRooms = FindObjectOfType<RoomsLayout>().Rooms;
        EncounterManager = FindObjectOfType<EncounterManager>();
        ConnectedRooms = new Tuple<RoomInfo, RoomInfo>(allRooms[Doors.ConnectingRooms[0]], allRooms[Doors.ConnectingRooms[1]]);
        allRooms[0].IsExploredChanged += RoomExplored;
        allRooms[1].IsExploredChanged += RoomExplored;
        UpdateDoorColor();
    }

    private void RoomExplored(object sender, bool e)
    {
        UpdateDoorColor();
    }

    void UpdateDoorColor()
    {
        // If one room is explored and the other is not, it means one the unexplored one will be the room where this door will lead.
        // So that encounter should be indicated by this door.
        if ( ConnectedRooms.Item1.IsExplored == ConnectedRooms.Item2.IsExplored)
        {
            return;
        }
        // The target room, the one which is the player yet to enter and whose difficulty this door should indicate, is the room that is not explored yet.
        var targetRoomIndex = ConnectedRooms.Item1.IsExplored ? Doors.ConnectingRooms[1] : Doors.ConnectingRooms[0];
        var targetEncounter = EncounterManager.EncounterConfigurations[targetRoomIndex];
        foreach (var indicator in DifficultyIndicators)
        { 
            if (indicator.EncounterDifficulty == targetEncounter.EncounterDifficulty)
            {
                var doorRenderers = transform.GetComponentsInChildren<SpriteRenderer>();
                foreach (var renderer in doorRenderers)
                {
                    renderer.color = indicator.Color;
                }
            }
        }
    }
}

[Serializable]
public struct DifficultyColorPair
{
    public Color Color;
    public EncounterDifficulty EncounterDifficulty;
}