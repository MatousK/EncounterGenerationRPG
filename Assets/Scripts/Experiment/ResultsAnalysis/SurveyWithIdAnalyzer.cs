using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Experiment.ResultsAnalysis
{
    class SurveyWithIdAnalyzer : SurveyAnalyzerBase
    {
        public SurveyWithIdAnalyzer(ResultAnalysisConfiguration configuration) : base(configuration)
        {

        }

        protected override string GetTargetDirectory(List<string> cells, string[] allDirectories)
        {
            var id = cells[5];
            foreach (var directory in allDirectories)
            {
                var directoryName = Path.GetFileName(directory);
                if (directoryName == id)
                {
                    return directory;
                }
            }
            UnityEngine.Debug.LogError($"No result found for id {id}");
            return null;
        }
    }
}
