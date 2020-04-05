using Assets.ProceduralLevelGenerator.Scripts.Pipeline;
using UnityEngine;

namespace Assets.Scripts.DungeonGenerator
{
    [CreateAssetMenu(menuName = "Dungeon generator/Encounter Generation RPG/Spawn Object", fileName = "SpawnObject")]
    public class SpawnObjectConfig : PipelineConfig
    {
        public GameObject ObjectToSpawn;
        public Vector3 ObjectPositon;
    }
}