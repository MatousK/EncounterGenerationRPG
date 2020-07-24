using Assets.ProceduralLevelGenerator.Scripts.Pipeline;
using UnityEngine;

namespace Assets.Scripts.DungeonGenerator
{
    /// <summary>
    /// Configuration for the task which adds the information about the room layout to the generated map, see <see cref="RoomExporterTask{TPayload}"/>.
    /// Needed because we need to know at runtime which rooms are which, what encounters should spawn there etc.
    /// This complex architecture is required by the library to allow the tasks to be generic.
    /// </summary>
    [CreateAssetMenu(menuName = "Dungeon generator/Encounter Generation RPG/Room Exporter", fileName = "RoomExporter")]
    public class RoomExporterConfig : PipelineConfig
    {
    }
}
