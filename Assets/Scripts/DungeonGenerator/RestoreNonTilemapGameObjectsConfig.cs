using Assets.ProceduralLevelGenerator.Scripts.Pipeline;
using UnityEngine;

namespace Assets.Scripts.DungeonGenerator
{
    /// <summary>
    /// Configuration for the task which adds to the generated map all game objects which were originally in the room, see <see cref="RestoreNonTilemapGameObjectsTask{TPayload}"/>.
    /// This complex architecture is required by the library to allow the tasks to be generic.
    /// </summary>
    [CreateAssetMenu(menuName = "Dungeon generator/Encounter Generation RPG/Restore non tile map objects task", fileName = "RestoreObjectsTask")]
    public class RestoreNonTilemapGameObjectsConfig : PipelineConfig
    {

    }
}