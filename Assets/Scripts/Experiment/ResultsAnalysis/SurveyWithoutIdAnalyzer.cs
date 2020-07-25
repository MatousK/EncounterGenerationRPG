using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Experiment.ResultsAnalysis
{
    /// <summary>
    /// <inheritdoc/>
    /// This class tries to match the survey by time. We assume that there should not be a large delay between the last event being sent to the server and survey start.
    /// However, this is not guaranteed to be accurate.
    /// We also do not have info about time zones, so we assume UTC+2, as that was the timezone in the Czech Republic at the start of the experiment, where we got most of our respondents.
    /// </summary>
    class SurveyWithoutIdAnalyzer : SurveyAnalyzerBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="SurveyWithoutIdAnalyzer"/>
        /// </summary>
        /// <param name="configuration"><inheritdoc/></param>
        public SurveyWithoutIdAnalyzer(ResultAnalysisConfiguration configuration) : base(configuration)
        {

        }
        /// <summary>
        /// How many surveys did we fail to assign. These are stored in a separate folder.
        /// </summary>
        private int unasignedSurveyIndex;
        /// <summary>
        /// <inheritdoc/>
        /// For each folder, reads its CSV data and finds the last entry. 
        /// We assume that for each CSV line there should be on of these last entries for which the time of sending is similar to the start time of the survey line.
        /// That is because we assume that people usually stop the experiment after being killed, which is a logged event.
        /// </summary>
        /// <param name="cells"><inheritdoc/></param>
        /// <param name="allDirectories"><inheritdoc/></param>
        /// <returns><inheritdoc/></returns>
        protected override string GetTargetDirectory(List<string> cells, string[] allDirectories)
        {
            // Ugly, hardcoded column index.
            var startDateString = cells[1];
            DateTime surveyStartDateTime;
            var culture = CultureInfo.GetCultureInfo("en");
            var styles = DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeLocal;
            if (!DateTime.TryParse(startDateString, culture, styles, out surveyStartDateTime))
            {
                UnityEngine.Debug.LogError("Cannot parse date.");
                return null;
            }
            TimeSpan? shortestDifference = null;
            string shortestDifferenceDirectory = null;
            foreach (var directory in allDirectories)
            {
                var rawDataPath = directory + "/" + Configuration.ProcessedRawDataFileName;
                if (!File.Exists(rawDataPath))
                {
                    continue;
                }
                var experimentEndDateTime = GetTestEndDateTime(rawDataPath);
                if (experimentEndDateTime == null)
                {
                    continue;
                }
                // We allow negative values, as time on server and local PC might not be in sync.
                var difference = surveyStartDateTime - experimentEndDateTime.Value;
                if (!shortestDifference.HasValue || difference.Duration() < shortestDifference)
                {
                    shortestDifference = difference.Duration();
                    shortestDifferenceDirectory = directory;
                }
            }
            UnityEngine.Debug.Log($"Shortest difference found: {shortestDifference}");
            UnityEngine.Debug.Log($"Shortest difference directory: {shortestDifferenceDirectory}");
            if (shortestDifference.Value.Minutes <= 2)
            {
                return shortestDifferenceDirectory;
            }
            else
            {
                // We think that more than a couple of minutes is too large of a difference to be explained by mismatched clocks. So we save the survey in a different location.
                return Configuration.ResultsRootDirectory + Configuration.UnassignedSurveyFolder + (unasignedSurveyIndex++).ToString();
            }

        }
        /// <summary>
        /// Retrieve the last logged entry for the session in the specified folder. That should be close to the end of the experiment.
        /// </summary>
        /// <param name="rawDataPath">Path where we can find the CSV line from which we want the last row.</param>
        /// <returns>The date when this finished ended, or null if this time could not be determined.</returns>
        private DateTime? GetTestEndDateTime(string rawDataPath)
        {
            // Find the last row and get it's time stamp. This should get us the approximate time when the user closed the game.
            string lastLine = null;
            using (StreamReader sr = new StreamReader(rawDataPath))
            {
                string currentLine = sr.ReadLine();
                while (currentLine != null)
                {
                    lastLine = currentLine;
                    currentLine = sr.ReadLine();
                }
            }
            if (lastLine == null)
            {
                return null;
            }
            var lastLineCells = lastLine.Split(';');
            var timeStampString = lastLineCells[2];
            var timeStampLong = long.Parse(timeStampString);
            // We add two hours to get to the proper timezone. Unfortunately we do not have the information about the timezone in the survey results.
            return DateTime.FromFileTimeUtc(timeStampLong).AddHours(2);
        }
    }
}
