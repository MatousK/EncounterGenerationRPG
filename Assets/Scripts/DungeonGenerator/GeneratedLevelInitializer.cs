using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.ProceduralLevelGenerator.Scripts.GeneratorPipeline.DungeonGenerators;
using UnityEngine;

namespace Assets.Scripts.DungeonGenerator
{
    class GeneratedLevelInitializer: MonoBehaviour
    {
        public List<GameObject> ObjectToTurnOnAfterGenerating;

        public DungeonGeneratorPipeline Generator;

        public void Start()
        {
            Generator.Generate();
            foreach (var gameObject in ObjectToTurnOnAfterGenerating)
            {
                gameObject.SetActive(true);
            }
        }
    }
}
