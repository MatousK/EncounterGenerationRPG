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
        protected override string GetTargetDirectory(string[] cells, string[] allDirectories, string processedResultsRootDirectory)
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
