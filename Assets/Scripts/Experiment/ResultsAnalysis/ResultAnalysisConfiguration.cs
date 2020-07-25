using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Experiment.ResultsAnalysis
{
    /// <summary>
    /// Set up paths for the results analysis.
    /// </summary>
    [Serializable]
    public class ResultAnalysisConfiguration
    {
        /// <summary>
        /// Which versions of the experiment should be included in the summary.
        /// </summary>
        public string[] AllowedVersions = new string[] { "v2" };
        /// <summary>
        /// Which number of levels must the player have to have completed to be a part of the results summary.
        /// </summary>
        public string[] AllowedLevelsFinished = new string[] { "5" };

        /// <summary>
        /// Name of the file name where the end survey of some specific player should be saved.
        /// </summary>
        public string ProcessedSurveyCompleteFileName = "EndSurvey.csv";
        /// <summary>
        /// Name of the file name where the half survey of some specific player should be saved.
        /// </summary>
        public string ProcessedSurveyHalfFileName = "HalfSurvey.csv";
        /// <summary>
        /// Name of the file name where the unfinished survey of some specific player should be saved.
        /// </summary>
        public string ProcessedUnfinishedFileName = "Unfinished.csv";
        /// <summary>
        /// Name of the file where we will save the analytics lines relevant to a single player.
        /// </summary>
        public string ProcessedRawDataFileName = "rawData.csv";
        /// <summary>
        /// The path where we should store the results of each individual player.
        /// </summary>
        public string ResultsRootDirectory = "Results/Processed/IndividualTests/";
        /// <summary>
        /// Where should we store the summary file.
        /// </summary>
        public string SummaryFilePath = "Results/Processed/Summary/SuccessfulUsersSummary.csv";
        /// <summary>
        /// Where should we store the end surveys of players we cannot match to any other data. Relative to the <see cref="ResultsRootDirectory"/>
        /// </summary>
        public string UnassignedSurveyFolder = $"/UnassignedSurvey/";

        /// <summary>
        /// The root folder where the unprocessed data are stored.
        /// </summary>
        public string RawResultsDirectory = "Results/Raw";
        /// <summary>
        /// File name with end surveys of players from the generated first group. Relative to the <see cref="RawResultsDirectory"/>
        /// </summary>
        public string GeneratedFirstCompletePath = "/GeneratedFirstComplete.csv";
        /// <summary>
        /// File name with halftime surveys of players from the generated first group. Relative to the <see cref="RawResultsDirectory"/>
        /// </summary>
        public string GeneratedFirstHalfPath = "/GeneratedFirstHalf.csv";
        /// <summary>
        /// File name with the surveys of players who did not finish the game. Relative to the <see cref="RawResultsDirectory"/>
        /// </summary>
        public string PrematureExitPath = "/Unfinished.csv";
        /// <summary>
        /// File name with end surveys of players from the static first group. Relative to the <see cref="RawResultsDirectory"/>
        /// </summary>
        public string StaticFirstCompletePath = "/StaticFirstComplete.csv";
        /// <summary>
        /// File name with end halftime of players from the static first group. Relative to the <see cref="RawResultsDirectory"/>
        /// </summary>
        public string StaticFirstHalfPath = "/StaticFirstHalf.csv";
        /// <summary>
        /// File name with the raw analytics data for all players. Relative to the <see cref="RawResultsDirectory"/>
        /// </summary>
        public string GeneralDataFilePath =  "/data.csv";

        /// <summary>
        /// String identifier for the experiment group who played the generated levels first.
        /// </summary>
        public string GeneratedFirstGroupName = "GeneratedFirst";
        /// <summary>
        /// String identifier for the experiment group who played the static levels first.
        /// </summary>
        public string StaticFirstGroupName = "StaticFirst";
        /// <summary>
        /// String identifier for the experiment group who did not finish the tutorial level.
        /// </summary>
        public string TutorialOnlyGroupName = "TutorialOnly";
        /// <summary>
        /// String identifier for the experiment group who encountered the bug in the V1 experiment which screwed up the matrix.
        /// </summary>
        public string InvalidValuesGroupName = "InvalidValues";
    }
}
