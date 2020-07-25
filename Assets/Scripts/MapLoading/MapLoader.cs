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
    /// <summary>
    /// This component generated the map when started.
    /// It uses the configuration from the <see cref="LevelLoader"/> to determine what graph to generate.
    /// Also, some of the game objects require the map to exist before doing anything.
    /// These start in the scene disabled.
    /// They are activated by this component when the map finishes loading.
    /// </summary>
    public class MapLoader: MonoBehaviour
    {
        /// <summary>
        /// Objects that should be activated once the map finishes loading.
        /// </summary>
        public List<GameObject> ObjectToActivateOnLoad;
        /// <summary>
        /// The pipeline that can generate the dungeon.
        /// </summary>
        public DungeonGeneratorPipeline GeneratorPipelineToRun; 
        /// <summary>
        /// Called before the first Update of this component.
        /// Creates the map.
        /// Can block the UI for a short while if the graph is too complicated, as the task is synchronous.
        /// </summary>
        public void Start()
        {
            // Find the level loader which has information about the graph we should generate.
            var levelLoader = FindObjectsOfType<LevelLoader>().FirstOrDefault(loader => !loader.IsPendingKill);
            // Set the graph we will be generating.
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
            // Generate the level.
            GeneratorPipelineToRun.Generate();
            // The level is generated. Activate the objects which require the map to exist.
            foreach (var objectToActivete in ObjectToActivateOnLoad)
            {
                objectToActivete.SetActive(true);
            }
        }
    }
}
