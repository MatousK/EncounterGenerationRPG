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
    /// <summary>
    /// The component which controls all result analysis.
    /// It is really hacky, we only needed some way to analyze the data, we do not expect anything from this namespace to be reused.
    /// So everything is quick, dirty and single use.
    /// It is also expected that this will freeze Unity every now and then.
    /// </summary>
    public class ResultsAnalyzer: MonoBehaviour
    {
        /// <summary>
        /// Configuration with input and output files for the analysis.
        /// </summary>
        public ResultAnalysisConfiguration Configuration;
        /// <summary>
        /// All the data of the players who did not revoke GDPR agreements, grouped by their user ID.
        /// </summary>
        List<IGrouping<string, CsvLine>> nonRevokedData;
        /// <summary>
        /// Index of the session currently being analyzed.
        /// </summary>
        int currentSessionIndex = 0;
        /// <summary>
        /// The object analyzing the current experiment session.
        /// </summary>
        SessionAnalyzer currentSessionAnalyzer;
        /// <summary>
        /// The initial encounter matrix, should be cloned for every reconstruction.
        /// </summary>
        EncounterDifficultyMatrix matrixTemplate;
        /// <summary>
        /// If true, we should reconstruct how the matrix looked for multiple users.
        /// This switch is here for debugging the analysis, as reconstructing the matrices and creating visualizations takes a long time.
        /// </summary>
        public bool ShouldReconstructMatrices;
        /// <summary>
        /// Called before the first update, parses the CSV lines of all players. Also, initializes the matrix.
        /// </summary>
        private void Start()
        {
            LoadGeneralData();
            var matrixProvider = gameObject.AddComponent<DifficultyMatrixProvider>();
            matrixProvider.ReloadMatrix(false);
            matrixTemplate = matrixProvider.CurrentDifficultyMatrix;
        }
        /// <summary>
        /// Called every frame. If there is not a session analysis in progress and we did not yet analyze all of them, analyze the next session.
        /// Once all session are analyzed, analyze the surveys and create the summary.
        /// Then self destruct, marking the analysis as completed.
        /// </summary>
        private void Update()
        {
            if (currentSessionAnalyzer == null && currentSessionIndex < nonRevokedData.Count)
            {
                var sessionAnalysisObject = new GameObject($"Session {currentSessionIndex}");
                currentSessionAnalyzer = sessionAnalysisObject.AddComponent<SessionAnalyzer>();
                currentSessionAnalyzer.Configuration = Configuration;
                currentSessionAnalyzer.ShouldReconstructMatrix = ShouldReconstructMatrices;
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
        /// <summary>
        /// Call to load and parse the CSV from the analytics backend.
        /// </summary>
        private void LoadGeneralData()
        {
            var generalData = new GeneralDataParser().LoadGeneralData(Configuration.RawResultsDirectory + Configuration.GeneralDataFilePath);
            var dataGroupedByUser = generalData.GroupBy(line => line.UserId);
            int i = dataGroupedByUser.Count();
            nonRevokedData = dataGroupedByUser.Where(userLines => !userLines.Any(line => line is AgreementRevokedLine)).ToList();
        }
        /// <summary>
        /// Analyzes all surveys filled out by the players.
        /// </summary>
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
