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
        const string ResultsPath = "Results/Raw";
        const string GeneratedFirstCompletePath = ResultsPath + "/GeneratedFirstComplete.csv";
        const string GeneratedFirstHalfPath = ResultsPath + "/GeneratedFirstHalf.csv";
        const string PrematureExitPath = ResultsPath + "/Unfinished.csv";
        const string StaticFirstCompletePath = ResultsPath + "/StaticFirstComplete.csv";
        const string StaticFirstHalfPath = ResultsPath + "/StaticFirstHalf.csv";
        const string GeneralDataFilePath = ResultsPath + "/data.csv";
        const string ResultsRootDirectory = "Results/Processed/IndividualTests/";

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
                currentSessionAnalyzer.ShouldReconstructMatrix = ShouldReconstructMatrices;
                currentSessionAnalyzer.ResultsRootDirectory = ResultsRootDirectory;
                var currentData = nonRevokedData[currentSessionIndex++];
                currentSessionAnalyzer.AnalyzeSession(currentData.ToList(), matrixTemplate.Clone());
            } 
            else if (currentSessionAnalyzer == null)
            {
                AnalyzeSurveys();
                Destroy(gameObject);
            }
        }

        private void LoadGeneralData()
        {
            var generalData = new GeneralDataParser().LoadGeneralData(GeneralDataFilePath);
            var dataGroupedByUser = generalData.GroupBy(line => line.UserId);
            int i = dataGroupedByUser.Count();
            nonRevokedData = dataGroupedByUser.Where(userLines => !userLines.Any(line => line is AgreementRevokedLine)).ToList();
            var matrixProvider = gameObject.AddComponent<DifficultyMatrixProvider>();
            matrixProvider.ReloadMatrix(false);
            matrixTemplate = matrixProvider.CurrentDifficultyMatrix;
        }

        private void AnalyzeSurveys()
        {
            var surveyWithIdAnalyzer = new SurveyWithIdAnalyzer();
            surveyWithIdAnalyzer.AnalyzeSurvey(GeneratedFirstCompletePath, ResultsRootDirectory, "GeneratedFirstComplete.csv");
            surveyWithIdAnalyzer.AnalyzeSurvey(GeneratedFirstHalfPath, ResultsRootDirectory, "GeneratedFirstHalf.csv");
            surveyWithIdAnalyzer.AnalyzeSurvey(StaticFirstCompletePath, ResultsRootDirectory, "StaticFirstComplete.csv");
            surveyWithIdAnalyzer.AnalyzeSurvey(StaticFirstHalfPath, ResultsRootDirectory, "StaticFirstHalf.csv");
            var surveyWithoutIdAnalyzer = new SurveyWithoutIdAnalyzer();
            surveyWithoutIdAnalyzer.AnalyzeSurvey(PrematureExitPath, ResultsRootDirectory, "PrematureExit.csv");
        }
    }
}
