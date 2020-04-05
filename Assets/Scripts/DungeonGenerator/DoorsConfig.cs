using Assets.ProceduralLevelGenerator.Scripts.Pipeline;
using UnityEngine;

namespace Assets.Scripts.DungeonGenerator
{
    [CreateAssetMenu(menuName = "Dungeon generator/Encounter Generation RPG/Doors task", fileName = "DoorsTask")]
    public class DoorsConfig : PipelineConfig
    {
        public bool AddDoors;

        public GameObject VerticalDoorsTop;

        public GameObject VerticalDoorsBottom;

        public GameObject HorizontalDoorsLeft;

        public GameObject HorizontalDoorsRight;
    }
}