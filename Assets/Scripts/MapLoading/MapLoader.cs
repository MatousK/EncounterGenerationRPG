using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.ProceduralLevelGenerator.Scripts.GeneratorPipeline.DungeonGenerators;
using Assets.ProceduralLevelGenerator.Scripts.GeneratorPipeline.InputSetup;
using Assets.Scripts.GameFlow;
using UnityEngine;

namespace Assets.Scripts.MapLoading
{
    public class MapLoader: MonoBehaviour
    {
        public List<GameObject> ObjectToActivateOnLoad;
        public DungeonGeneratorPipeline GeneratorPipelineToRun; 

        public void Start()
        {
            var levelLoader = FindObjectOfType<LevelLoader>();
            if (levelLoader != null && levelLoader.CurrentLevelGraph != null)
            {
                foreach (var pipelineItem in GeneratorPipelineToRun.PipelineItems)
                {
                    if (pipelineItem is FixedInputConfig)
                    {
                        (pipelineItem as FixedInputConfig).LevelGraph = levelLoader.CurrentLevelGraph;
                    }
                }
            }
            GeneratorPipelineToRun.Generate();
            foreach (var objectToActivete in ObjectToActivateOnLoad)
            {
                objectToActivete.SetActive(true);
            }
        }
    }
}
