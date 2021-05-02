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
    /// <summary>
    /// Everything the level loader needs to know about a level to load it.
    /// </summary>
    [CreateAssetMenu(menuName = "Encounter generator/Level Definition", fileName = "LevelDefinition")]
    public class LevelDefinition: ScriptableObject
    {
        /// <summary>
        /// In tutorial levels, doors should not be colored.
        /// </summary>
        public bool IsTutorialLevel;
        /// <summary>
        /// If true, doors should use alternate colors to indicate difficulty. 
        /// </summary>
        public bool UseAlternateDoorColors;
        /// <summary>
        /// If true, difficulty matrix will be updated even for static encounters. If false, only generated encounters will have an effect on the matrix.
        /// </summary>
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
        /// When this level is loaded, one of these graphs will be selected at random and used to generate the level.
        /// </summary>
        public LevelGraph[] PossibleLevelGraphs;
        /// <summary>
        /// If not empty, this text will be shown as an intro text.
        /// </summary>
        [TextArea(0,10)]
        public string[] IntroTexts;
        /// <summary>
        /// Settings which are dependent on the experiment group.
        /// We cannot use dictionary, as dictionaries are not visible in editor.
        /// </summary>
        public List<ExperimentConfiguration> ExperimentGroupConfigurations;
    }
    /// <summary>
    /// Information about a level which are relevant only to a certain experiment group.
    /// </summary>
    [Serializable]
    public class ExperimentConfiguration
    {
        /// <summary>
        /// The experiment group for which these settings apply.
        /// </summary>
        public ExperimentGroup ExperimentGroup;
        /// <summary>
        /// The algorithm that should be used in this level.
        /// </summary>
        public EncounterGenerationAlgorithmType Algorithm;
        /// <summary>
        /// If not empty, this is a link to the survey to be taken at the start of this level.
        /// </summary>
        public string SurveyLink;
    }
}
