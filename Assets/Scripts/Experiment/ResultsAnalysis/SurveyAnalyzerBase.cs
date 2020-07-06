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
        public void AnalyzeSurvey(string filePath, string processedResultsRoot, string outputFilename)
        {
            using (var sr = new StreamReader(filePath))
            {
                var allDirectories = GetAllProcessedFolders(processedResultsRoot);
                var headerLine = ReadCsvLineWithNewLines(sr);
                var currentLine = ReadCsvLineWithNewLines(sr);
                while (currentLine != null && currentLine.Any())
                {
                    var cells = currentLine.Split(',');
                    SaveSurveyLine(cells, allDirectories, currentLine, headerLine, outputFilename, processedResultsRoot);
                    currentLine = ReadCsvLineWithNewLines(sr);
                }
            }
        }

        private void SaveSurveyLine(string[] cells, string[] allDirectories, string line, string header, string outputFilename, string processedResultsRoot)
        {
            var targetDirectory = GetTargetDirectory(cells, allDirectories, processedResultsRoot);
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
        protected abstract string GetTargetDirectory(string[] cells, string[] allDirectories, string processedResultsRootDirectory);


        private string[] GetAllProcessedFolders(string processedResultsRoot)
        {
            return Directory.GetDirectories(processedResultsRoot, "*", SearchOption.AllDirectories);
        }

        private string ReadCsvLineWithNewLines(StreamReader reader)
        {
            // Ok, this is really hacky and won't work in all cases, but this is a quick way to get a line from csv line that might containg newlines in a cell between quotes.
            StringBuilder output = new StringBuilder();
            var buffer = new char[1];
            var isInQuotedCell = false;
            char? previousChar = null;
            while (!reader.EndOfStream)
            {
                reader.Read(buffer, 0, 1);
                var currentChar = buffer[0];
                if (previousChar == '"' && currentChar != '"')
                {
                    // Unescaped quote
                    isInQuotedCell = !isInQuotedCell;
                }
                if (currentChar == '\n' && !isInQuotedCell)
                {
                    break;
                }
                else if (currentChar != '\r')
                {
                    output.Append(currentChar);
                }
                previousChar = currentChar;
            }
            return output.ToString();
        }
    }
}
