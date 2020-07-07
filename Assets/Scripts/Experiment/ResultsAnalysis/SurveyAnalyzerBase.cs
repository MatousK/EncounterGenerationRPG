using Assets.Scripts.Extension;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Experiment.ResultsAnalysis
{
    abstract class SurveyAnalyzerBase
    {
        protected ResultAnalysisConfiguration Configuration;

        public SurveyAnalyzerBase(ResultAnalysisConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void AnalyzeSurvey(string filePath, string outputFilename)
        {
            using (var sr = new StreamReader(filePath))
            {
                var allDirectories = GetAllProcessedFolders(Configuration.ResultsRootDirectory);
                var headerLine = string.Join(",", sr.ReadCsvLine());
                var currentLineCells = sr.ReadCsvLine();
                var currentLine = string.Join(",", currentLineCells);
                while (currentLineCells != null && currentLineCells.Count > 1)
                {
                    SaveSurveyLine(currentLineCells, allDirectories, currentLine, headerLine, outputFilename);
                    currentLineCells = sr.ReadCsvLine();
                    currentLine = string.Join(",", currentLineCells);
                }
            }
        }

        private void SaveSurveyLine(List<string> cells, string[] allDirectories, string line, string header, string outputFilename)
        {
            var targetDirectory = GetTargetDirectory(cells, allDirectories);
            if (targetDirectory == null)
            {
                UnityEngine.Debug.LogError($"Could not save line.");
                return;
            }
            Directory.CreateDirectory(targetDirectory);
            using (var sw = new StreamWriter(targetDirectory + "/" + outputFilename))
            {
                sw.WriteLine(header);
                sw.WriteLine(line);
            }
        }
        protected abstract string GetTargetDirectory(List<string> cells, string[] allDirectories);


        private string[] GetAllProcessedFolders(string processedResultsRoot)
        {
            return Directory.GetDirectories(processedResultsRoot, "*", SearchOption.AllDirectories);
        }
    }
}
