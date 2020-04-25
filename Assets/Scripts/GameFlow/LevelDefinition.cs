using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.ProceduralLevelGenerator.Scripts.Data.Graphs;
using Assets.Scripts.Experiment;
using UnityEngine;

namespace Assets.Scripts.GameFlow
{
    [CreateAssetMenu(menuName = "Encounter generator/Level Definition", fileName = "LevelDefinition")]
    public class LevelDefinition: ScriptableObject
    {
        public bool AdjustMatrixForStaticEncounters;
        /// <summary>
        /// If true, at the beginning of this level we should use stats from after the tutorial instead of after the previous level.
        /// Used to restart the stats between experiment fazes. 
        /// </summary>
        public bool ShouldRestoreAfterTutorialStats;
        /// <summary>
        /// The scene to load for this level.
        /// </summary>
        public SceneType Type;
        /// <summary>
        /// When this level is loaded, one of these graphs will be used to generate the level.
        /// </summary>
        public LevelGraph[] PossibleLevelGraphs;
        /// <summary>
        /// If not empty, this text will be shown as an intro text.
        /// </summary>
        [TextArea(0,10)]
        public string[] IntroTexts;

        public List<ExperimentConfiguration> ExperimentGroupConfigurations;
    }

    [Serializable]
    public class ExperimentConfiguration
    {
        public ExperimentGroup ExperimentGroup;
        public EncounterGenerationAlgorithmType Algorithm;
        /// <summary>
        /// If not empty, this is a link to the survey to be taken at the start of this level.
        /// </summary>
        public String SurveyLink;
    }
}
