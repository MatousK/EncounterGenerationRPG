using Assets.ProceduralLevelGenerator.Scripts.GeneratorPipeline.Payloads.Interfaces;
using Assets.ProceduralLevelGenerator.Scripts.Pipeline;
using UnityEngine;

namespace Assets.Scripts.DungeonGenerator
{
    /// <summary>
    /// Adds the specified object to the map. Uses <see cref="SpawnObjectConfig"/> for configuration.
    /// </summary>
    /// <typeparam name="TPayload"><inheritdoc/></typeparam>
    public class SpawnObjectTask<TPayload> : ConfigurablePipelineTask<TPayload, SpawnObjectConfig>
        where TPayload : class, IGeneratorPayload, IGraphBasedGeneratorPayload, INamedTilemapsPayload
    {
        /// <summary>
        /// Spawns the specified object on the map.
        /// </summary>
        public override void Process()
        {
            var parentGameObject = Payload.GameObject;
            var newObject = Object.Instantiate(Config.ObjectToSpawn, parentGameObject.transform);
            newObject.transform.localPosition = Config.ObjectPositon;
        }
    }
}