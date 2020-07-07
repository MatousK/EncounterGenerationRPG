using Assets.Scripts.EncounterGenerator;
using Assets.Scripts.EncounterGenerator.Model;
using Assets.Scripts.Experiment.ResultsAnalysis.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Experiment.ResultsAnalysis
{
    public class ResultsAnalyzer: MonoBehaviour
    {
        public ResultAnalysisConfiguration Configuration;
        List<IGrouping<string, CsvLine>> nonRevokedData;
        int currentSessionIndex = 0;
        SessionAnalyzer currentSessionAnalyzer;
        EncounterDifficultyMatrix matrixTemplate;
        /// <summary>
        /// If true, we should reconstruct how the matrix looked for multiple users.
        /// This switch is here for debugging the experiment, as reconstructing the matrices takes a long time.
        /// </summary>
        public bool ShouldReconstructMatrices;
        public void Start()
        {
            LoadGeneralData();
        }

        public void Update()
        {
            if (currentSessionAnalyzer == null && currentSessionIndex < nonRevokedData.Count)
            {
                var sessionAnalysisObject = new GameObject($"Session {currentSessionIndex}");
                currentSessionAnalyzer = sessionAnalysisObject.AddComponent<SessionAnalyzer>();
                currentSessionAnalyzer.Configuration = Configuration;
                currentSessionAnalyzer.ShouldReconstructMatrix = ShouldReconstructMatrices;
                currentSessionAnalyzer.ResultsRootDirectory = Configuration.ResultsRootDirectory;
                var currentData = nonRevokedData[currentSessionIndex++];
                currentSessionAnalyzer.AnalyzeSession(currentData.ToList(), matrixTemplate.Clone());
            } 
            else if (currentSessionAnalyzer == null)
            {
                AnalyzeSurveys();
                var summaryHelper = new ExperimentSummarizationHelper(Configuration);
                summaryHelper.SummarizeExperiment();
                Destroy(gameObject);
            }
        }

        private void LoadGeneralData()
        {
            var generalData = new GeneralDataParser().LoadGeneralData(Configuration.RawResultsDirectory + Configuration.GeneralDataFilePath);
            var dataGroupedByUser = generalData.GroupBy(line => line.UserId);
            int i = dataGroupedByUser.Count();
            nonRevokedData = dataGroupedByUser.Where(userLines => !userLines.Any(line => line is AgreementRevokedLine)).ToList();
            var matrixProvider = gameObject.AddComponent<DifficultyMatrixProvider>();
            matrixProvider.ReloadMatrix(false);
            matrixTemplate = matrixProvider.CurrentDifficultyMatrix;
        }

        private void AnalyzeSurveys()
        {
            var surveyWithIdAnalyzer = new SurveyWithIdAnalyzer(Configuration);
            surveyWithIdAnalyzer.AnalyzeSurvey(Configuration.RawResultsDirectory + Configuration.GeneratedFirstCompletePath, Configuration.ProcessedSurveyCompleteFileName);
            surveyWithIdAnalyzer.AnalyzeSurvey(Configuration.RawResultsDirectory + Configuration.GeneratedFirstHalfPath, Configuration.ProcessedSurveyHalfFileName);
            surveyWithIdAnalyzer.AnalyzeSurvey(Configuration.RawResultsDirectory + Configuration.StaticFirstCompletePath, Configuration.ProcessedSurveyCompleteFileName);
            surveyWithIdAnalyzer.AnalyzeSurvey(Configuration.RawResultsDirectory + Configuration.StaticFirstHalfPath, Configuration.ProcessedSurveyHalfFileName);
            var surveyWithoutIdAnalyzer = new SurveyWithoutIdAnalyzer(Configuration);
            surveyWithoutIdAnalyzer.AnalyzeSurvey(Configuration.RawResultsDirectory + Configuration.PrematureExitPath, Configuration.ProcessedUnfinishedFileName);
        }
    }
}
