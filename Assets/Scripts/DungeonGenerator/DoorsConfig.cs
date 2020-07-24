using Assets.ProceduralLevelGenerator.Scripts.Pipeline;
using UnityEngine;

namespace Assets.Scripts.DungeonGenerator
{
    /// <summary>
    /// Configuration for the task which adds doors to the rooms, see <see cref="DoorsTask{TPayload}"/>.
    /// This complex architecture is required by the library to allow the tasks to be generic.
    /// Not used when using corridors.
    /// This class contains the parameters for the task which can be set in editor.
    /// </summary>
    [CreateAssetMenu(menuName = "Dungeon generator/Encounter Generation RPG/Doors task", fileName = "DoorsTask")]
    public class DoorsConfig : PipelineConfig
    {
        /// <summary>
        /// If true, doors should be added. Set to false to basically disable this task.
        /// </summary>
        public bool AddDoors;
        /// <summary>
        /// The game object representing the doors used when the doors are oriented to the top of the screen.
        /// </summary>
        public GameObject VerticalDoorsTop;
        /// <summary>
        /// The game object representing the doors used when the doors are oriented to the bottom of the screen.
        /// </summary>
        public GameObject VerticalDoorsBottom;
        /// <summary>
        /// The game object representing the doors used when the doors are oriented to the left side of the screen.
        /// </summary>
        public GameObject HorizontalDoorsLeft;
        /// <summary>
        /// The game object representing the doors used when the doors are oriented to the right side of the screen.
        /// </summary>
        public GameObject HorizontalDoorsRight;
    }
}