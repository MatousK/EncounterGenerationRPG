using Assets.ProceduralLevelGenerator.Scripts.Pipeline;
using UnityEngine;

namespace Assets.Scripts.DungeonGenerator
{
    /// <summary>
    /// Configuration for the task which spawns some specific game object on the map, see <see cref="SpawnObjectTask{TPayload}"/>
    /// This complex architecture is required by the library to allow the tasks to be generic.
    /// </summary>
    [CreateAssetMenu(menuName = "Dungeon generator/Encounter Generation RPG/Spawn Object", fileName = "SpawnObject")]
    public class SpawnObjectConfig : PipelineConfig
    {
        /// <summary>
        /// Object that should be spawned on the map.
        /// </summary>
        public GameObject ObjectToSpawn;
        /// <summary>
        /// Position where the object should be spawned.
        /// </summary>
        public Vector3 ObjectPositon;
    }
}