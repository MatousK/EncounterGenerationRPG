using Assets.Scripts.Extension;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Experiment.ResultsAnalysis
{
    /// <summary>
    /// This class can analyze the results of some survey.
    /// It first gets the list of all test results available. 
    /// Then it goes through the survey being analyzed line by line.
    /// If the line is matched to some output folder, save that line to that folder.
    /// </summary>
    abstract class SurveyAnalyzerBase
    {
        /// <summary>
        /// The configuration which specifies input and output files for the analysis.
        /// </summary>
        protected ResultAnalysisConfiguration Configuration;
        /// <summary>
        /// Initializes a new instance of the <see cref="SurveyAnalyzerBase"/> class.
        /// </summary>
        /// <param name="configuration">The configuration which specifies input and output files for the analysis.</param>
        public SurveyAnalyzerBase(ResultAnalysisConfiguration configuration)
        {
            Configuration = configuration;
        }
        /// <summary>
        /// Analyzes the survey with the specified file name.
        /// </summary>
        /// <param name="filePath">The path to the survey.</param>
        /// <param name="outputFilename">The name of the file where we should save the survey once we find the target test directory.</param>
        public void AnalyzeSurvey(string filePath, string outputFilename)
        {
            using (var sr = new StreamReader(filePath))
            {
                // Get the list of all directories created as a part of the survey analysis.
                var allDirectories = GetAllProcessedFolders(Configuration.ResultsRootDirectory);
                var headerLine = string.Join(",", sr.ReadCsvLine());
                // Go line by line through the survey and try to analyze the individual line.
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
            // Child classes specify how to get the target directory for each CSV line.
            var targetDirectory = GetTargetDirectory(cells, allDirectories);
            if (targetDirectory == null)
            {
                UnityEngine.Debug.LogError($"Could not save line.");
                return;
            }
            // Found target directory, create save the line there.
            Directory.CreateDirectory(targetDirectory);
            using (var sw = new StreamWriter(targetDirectory + "/" + outputFilename))
            {
                sw.WriteLine(header);
                sw.WriteLine(line);
            }
        }
        /// <summary>
        /// Retrieve the directory where we should save the specified CSV line.
        /// </summary>
        /// <param name="cells">List of all cells in the CSV line.</param>
        /// <param name="allDirectories">All directories with individual tests output files.</param>
        /// <returns>The path where we should save this survey line, or null if the survey result could not be matched.</returns>
        protected abstract string GetTargetDirectory(List<string> cells, string[] allDirectories);
        /// <summary>
        /// Retrieve the list of all individual processed tests folders.
        /// Each folder corresponds to a single session.
        /// </summary>
        /// <param name="processedResultsRoot">The path to the folder where the processed test results are stored.</param>
        /// <returns>list of all individual processed test folders.</returns>
        private string[] GetAllProcessedFolders(string processedResultsRoot)
        {
            return Directory.GetDirectories(processedResultsRoot, "*", SearchOption.AllDirectories);
        }
    }
}
