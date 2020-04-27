using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.DungeonGenerator;
using Assets.Scripts.EncounterGenerator.Configuration;
using Assets.Scripts.GameFlow;
using UnityEngine;

namespace Assets.Scripts.Environment
{
    public class DoorDifficultyIndicator : MonoBehaviour
    {
        public List<DifficultyColorPair> DifficultyIndicators;
        /// <summary>
        /// Used to mark the doors in phase two of an experiment.
        /// </summary>
        public List<DifficultyColorPair> AlternateDifficultyIndicators;
        private RoomInfo connectedRoom;
        Doors doors;
        private LevelLoader levelLoader;

        private void Start()
        {
            levelLoader = FindObjectsOfType<LevelLoader>().First(loader => !loader.IsPendingKill);
            doors = GetComponent<Doors>();
            var allRooms = FindObjectOfType<RoomsLayout>().Rooms;
            connectedRoom = allRooms[doors.ConnectingRooms[0]];
            UpdateDoorColor();
        }

        void UpdateDoorColor()
        {
            var indicators = levelLoader.UseAlternateDoorColors ? AlternateDifficultyIndicators : DifficultyIndicators;
            foreach (var indicator in indicators)
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