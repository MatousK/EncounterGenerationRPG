using Assets.ProceduralLevelGenerator.Scripts.GeneratorPipeline.Payloads.Interfaces;
using Assets.ProceduralLevelGenerator.Scripts.Pipeline;
using UnityEngine;

namespace Assets.Scripts.DungeonGenerator
{
    public class SpawnObjectTask<TPayload> : ConfigurablePipelineTask<TPayload, SpawnObjectConfig>
        where TPayload : class, IGeneratorPayload, IGraphBasedGeneratorPayload, INamedTilemapsPayload
    {
        public override void Process()
        {
            var parentGameObject = Payload.GameObject;
            var newObject = Object.Instantiate(Config.ObjectToSpawn, parentGameObject.transform);
            newObject.transform.localPosition = Config.ObjectPositon;
        }
    }
}