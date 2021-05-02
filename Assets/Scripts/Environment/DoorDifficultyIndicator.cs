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
    /// <summary>
    /// When added to an object that also has a <see cref="Doors"/> component, this class colors the doors based on their difficulty.
    /// </summary>
    public class DoorDifficultyIndicator : MonoBehaviour
    {
        /// <summary>
        /// For each difficulty, what color should be used for them in the first and second phase of the experiment.
        /// </summary>
        public List<DifficultyColorPair> DifficultyIndicators;
        /// <summary>
        /// For each difficulty, what color should be used in the third phase of the experiment.
        /// </summary>
        public List<DifficultyColorPair> AlternateDifficultyIndicators;
        /// <summary>
        /// The room to which this door leads. Always only one, as we are not interested in the corridor on the other side of the door.
        /// </summary>
        private RoomInfo connectedRoom;
        /// <summary>
        /// Doors attached to this object.
        /// </summary>
        Doors doors;
        /// <summary>
        /// Class responsible for loading of levels. Knows whether we are in the first, second or third phase of the experiment.
        /// </summary>
        private LevelLoader levelLoader;

        private void Start()
        {
            levelLoader = FindObjectsOfType<LevelLoader>().First(loader => !loader.IsPendingKill);
            doors = GetComponent<Doors>();
            var allRooms = FindObjectOfType<RoomsLayout>().Rooms;
            connectedRoom = allRooms[doors.ConnectingRooms[0]];
            UpdateDoorColor();
        }
        /// <summary>
        /// Tries to find the correct door for these doors and apply it to the sprite renderers rendering the doors.
        /// If no color is found for these doors, the method does nothing.
        /// </summary>
        void UpdateDoorColor()
        {
            if (levelLoader.IsTutorialLevel)
            {
                // In tutorial levels, we should not modify door colors.
                return;
            }
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
    /// <summary>
    /// For a difficulty specifies what color should the doors use.
    /// </summary>
    [Serializable]
    public struct DifficultyColorPair
    {
        /// <summary>
        /// Color the doors should use.
        /// </summary>
        public Color Color;
        /// <summary>
        /// The difficulty that should use this color.
        /// </summary>
        public EncounterDifficulty EncounterDifficulty;
    }
}