using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Experiment.ResultsAnalysis
{
    [Serializable]
    public class ResultAnalysisConfiguration
    {
        public string[] AllowedVersions = new string[] { "v2" };
        public string[] AllowedLevelsFinished = new string[] { "5" };

        public string ProcessedSurveyCompleteFileName = "EndSurvey.csv";
        public string ProcessedSurveyHalfFileName = "HalfSurvey.csv";
        public string ProcessedUnfinishedFileName = "Unfinished.csv";
        public string ProcessedRawDataFileName = "rawData.csv";
        public string ResultsRootDirectory = "Results/Processed/IndividualTests/";
        public string SummaryFilePath = "Results/Processed/Summary/SuccessfulUsersSummary.csv";

        public string UnassignedSurveyFolder = $"/UnassignedSurvey/";

        public string RawResultsDirectory = "Results/Raw";
        public string GeneratedFirstCompletePath = "/GeneratedFirstComplete.csv";
        public string GeneratedFirstHalfPath = "/GeneratedFirstHalf.csv";
        public string PrematureExitPath = "/Unfinished.csv";
        public string StaticFirstCompletePath = "/StaticFirstComplete.csv";
        public string StaticFirstHalfPath = "/StaticFirstHalf.csv";
        public string GeneralDataFilePath =  "/data.csv";

        public string GeneratedFirstGroupName = "GeneratedFirst";
        public string StaticFirstGroupName = "StaticFirst";
        public string TutorialOnlyGroupName = "TutorialOnly";
        public string InvalidValuesGroupName = "InvalidValues";
    }
}
