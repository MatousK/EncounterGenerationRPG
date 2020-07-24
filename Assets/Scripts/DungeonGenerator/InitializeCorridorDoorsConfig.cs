using Assets.ProceduralLevelGenerator.Scripts.Pipeline;
using UnityEngine;

namespace Assets.Scripts.DungeonGenerator
{
    /// <summary>
    /// Configuration for the task which initializes corridors to the rooms, see <see cref="InitializeCorridorDoorsTask{TPayload}"/>.
    /// The task assigns to the doors info about which rooms they connect.
    /// This complex architecture is required by the library to allow the tasks to be generic.
    /// </summary>
    [CreateAssetMenu(menuName = "Dungeon generator/Encounter Generation RPG/Initialize Corridor Doors task", fileName = "InitCorridorDoors")]
    class InitializeCorridorDoorsConfig : PipelineConfig
    {
    }
}