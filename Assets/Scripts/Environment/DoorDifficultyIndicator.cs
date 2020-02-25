using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class DoorDifficultyIndicator : MonoBehaviour
{
    public List<DifficultyColorPair> DifficultyIndicators;
    RoomInfo ConnectedRoom;
    Doors Doors;

    private void Start()
    {
        Doors = GetComponent<Doors>();
        var allRooms = FindObjectOfType<RoomsLayout>().Rooms;
        ConnectedRoom = allRooms[Doors.ConnectingRooms[0]];
        UpdateDoorColor();
    }

    void UpdateDoorColor()
    {
        foreach (var indicator in DifficultyIndicators)
        { 
            if (indicator.EncounterDifficulty == ConnectedRoom.RoomEncounter.EncounterDifficulty)
            {
                var doorRenderers = transform.GetComponentsInChildren<SpriteRenderer>(true);
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