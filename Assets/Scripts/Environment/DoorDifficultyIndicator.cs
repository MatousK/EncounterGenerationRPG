using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.DungeonGenerator;
using Assets.Scripts.EncounterGenerator.Configuration;
using UnityEngine;

namespace Assets.Scripts.Environment
{
    public class DoorDifficultyIndicator : MonoBehaviour
    {
        public List<DifficultyColorPair> DifficultyIndicators;
        private RoomInfo connectedRoom;
        Doors doors;

        private void Start()
        {
            doors = GetComponent<Doors>();
            var allRooms = FindObjectOfType<RoomsLayout>().Rooms;
            connectedRoom = allRooms[doors.ConnectingRooms[0]];
            UpdateDoorColor();
        }

        void UpdateDoorColor()
        {
            foreach (var indicator in DifficultyIndicators)
            { 
                if (indicator.EncounterDifficulty == connectedRoom.RoomEncounter.EncounterDifficulty)
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
}