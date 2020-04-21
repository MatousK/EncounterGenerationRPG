using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.ProceduralLevelGenerator.Scripts.GeneratorPipeline.DungeonGenerators;
using UnityEngine;

namespace Assets.Scripts.MapLoading
{
    public class MapLoader: MonoBehaviour
    {
        public List<GameObject> ObjectToActivateOnLoad;
        public DungeonGeneratorPipeline GeneratorPipelineToRun; 

        public void Start()
        {
            GeneratorPipelineToRun.Generate();
            foreach (var objectToActivete in ObjectToActivateOnLoad)
            {
                objectToActivete.SetActive(true);
            }
        }
    }
}
