using Assets.Scripts.Editor;
using Assets.Scripts.Editor.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

namespace Assets.Scripts.Experiment
{
    public class ResultsAnalyzer
    {
        const string ResultsPath = "Results/Raw";
        const string GeneratedFirstDonePath = ResultsPath + "/GeneratedFirstDone.csv";
        const string GeneratedFirstHalfPath = ResultsPath + "/GeneratedFirstHalf.csv";
        const string PrematureExitPath = ResultsPath + "/PrematureExit.csv";
        const string StaticFirstDonePath = ResultsPath + "/StaticFirstDone.csv";
        const string StaticFirstHalfPath = ResultsPath + "/StaticFirstHalf.csv";
        const string GeneralDataFilePath = ResultsPath + "/data.csv";
        [MenuItem("Thesis/Start Analysis")]
        public static void StartAnalysis()
        {

        }

        private static void LoadGeneralData()
        {
            var generalData = (new GeneralDataParser()).LoadGeneralData(GeneralDataFilePath);
            var dataGroupedByUser = generalData.GroupBy(line => line.UserId);
            var nonRevokedData = dataGroupedByUser.Where(userLines => !userLines.Any(line => line is AgreementRevokedLine));
            
        }
    }
}
