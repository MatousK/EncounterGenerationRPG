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
        const string GeneratedFirstDonePath = ResultsPath + "/GeneratedFirstDone.csv";
        const string GeneratedFirstHalfPath = ResultsPath + "/GeneratedFirstHalf.csv";
        const string PrematureExitPath = ResultsPath + "/PrematureExit.csv";
        const string StaticFirstDonePath = ResultsPath + "/StaticFirstDone.csv";
        const string StaticFirstHalfPath = ResultsPath + "/StaticFirstHalf.csv";
        const string GeneralDataFilePath = ResultsPath + "/data.csv";

        List<IGrouping<string, CsvLine>> nonRevokedData;
        int currentSessionIndex = 0;
        SessionAnalyzer currentSessionAnalyzer;
        EncounterDifficultyMatrix matrixTemplate;
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
                var currentData = nonRevokedData[currentSessionIndex++];
                currentSessionAnalyzer.AnalyzeSession(currentData.ToList(), matrixTemplate.Clone());
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
    }
}
