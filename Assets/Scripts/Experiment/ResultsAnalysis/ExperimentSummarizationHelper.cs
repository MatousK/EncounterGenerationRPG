using Assets.Scripts.Experiment.ResultsAnalysis.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Extension;

namespace Assets.Scripts.Experiment.ResultsAnalysis
{
    /// <summary>
    /// This class can summarize the experiment for all successful user and create a single CSV file.
    /// It uses the data saved on a disk during partial analyses.
    /// </summary>
    class ExperimentSummarizationHelper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExperimentSummarizationHelper"/> class.
        /// </summary>
        /// <param name="configuration">The configuration of the result analysis.</param>
        public ExperimentSummarizationHelper(ResultAnalysisConfiguration configuration)
        {
            this.configuration = configuration;
        }
        /// <summary>
        /// Configuration specifying things like output paths etc.
        /// </summary>
        ResultAnalysisConfiguration configuration;
        /// <summary>
        /// Creates the summarization file of the experiment.
        /// </summary>
        public void SummarizeExperiment()
        {
            // Not pretty, but basically, traverse the structure of the experiment results and get to the valid results of tests.
            // But first, create the output file.
            var summaryDirectory = Path.GetDirectoryName(configuration.SummaryFilePath);
            Directory.CreateDirectory(summaryDirectory);
            using (var outputWriter = new StreamWriter(configuration.SummaryFilePath))
            {
                outputWriter.WriteLine("sep=;");
                ExperimentSummaryCsvLine.WriteHeader(outputWriter);
                var resultsRoot = configuration.ResultsRootDirectory;
                // Filter only allowed versions of the experiment relevant to this analysis.
                var versionFolders = configuration.AllowedVersions.Select(version => resultsRoot + version);
                foreach (var versionFolder in versionFolders)
                {
                    SummarizeVersionFolder(versionFolder, outputWriter);
                }
            }
        }
        /// <summary>
        /// Found a valid version, summarize all subfolders.
        /// </summary>
        /// <param name="versionFolder">The valid folder to analyze.</param>
        /// <param name="outputWriter">The output file.</param>
        private void SummarizeVersionFolder(string versionFolder, StreamWriter outputWriter)
        {
            if (!Directory.Exists(versionFolder))
            {
                return;
            }
            var directories = Directory.GetDirectories(versionFolder, "*", SearchOption.TopDirectoryOnly);
            foreach (var directory in directories)
            {
                var groupName = Path.GetFileName(directory);
                // Filter only the players who finished some number of levels defined in the configuration.
                var levelsFinishedFolders = configuration.AllowedLevelsFinished.Select(version => directory + "/" + version);
                foreach (var levelsFinishedFolder in levelsFinishedFolders)
                {
                    SummarizeLevelsFinishedFolder(groupName, levelsFinishedFolder, outputWriter);
                }
            }
        }
        /// <summary>
        /// Now we have folder with results of users with valid sections and versions.
        /// </summary>
        /// <param name="groupName">Name of the group of players in this folder.</param>
        /// <param name="folder">Current folder.</param>
        /// <param name="outputWriter">Output file.</param>
        private void SummarizeLevelsFinishedFolder(string groupName, string folder, StreamWriter outputWriter)
        {
            if (!Directory.Exists(folder))
            {
                return;
            }
            var directories = Directory.GetDirectories(folder, "*", SearchOption.TopDirectoryOnly);
            foreach (var singleTestResultDirectory in directories)
            {
                var userId = Path.GetFileName(singleTestResultDirectory);
                SummarizeSingleTest(groupName, userId, singleTestResultDirectory, outputWriter);
            }
        }
        /// <summary>
        /// Finally we got to a folder with the results of a single test.
        /// Summarize the results saved in this folder.
        /// </summary>
        /// <param name="groupName">Name of the group of this player.</param>
        /// <param name="userId">User id of the player.</param>
        /// <param name="folder">Current folder.</param>
        /// <param name="outputWriter">Output file.</param>
        private void SummarizeSingleTest(string groupName, string userId, string folder, StreamWriter outputWriter)
        {
            // Create the summary line, fill it with data and save it to output.
            var summaryLine = new ExperimentSummaryCsvLine();
            summaryLine.Group = groupName;
            summaryLine.UserId = userId;
            // If true, this is a player who played the generated encounters first.
            bool isGeneratedFirstGroup = groupName == configuration.GeneratedFirstGroupName;

            var firstHalfFileName = folder + "/" + configuration.ProcessedSurveyHalfFileName;
            FillExperimentHalfSummary(firstHalfFileName, true, isGeneratedFirstGroup, summaryLine);

            var completeFileName = folder + "/" + configuration.ProcessedSurveyCompleteFileName;
            FillExperimentHalfSummary(completeFileName, false, isGeneratedFirstGroup, summaryLine);

            FillDemographics(completeFileName, summaryLine);

            var rawDataFileName = folder + "/" + configuration.ProcessedRawDataFileName;
            FillCombatResultsSummary(rawDataFileName, summaryLine);

            summaryLine.WriteLine(outputWriter);
        }
        /// <summary>
        /// Analyze the survey response of the player.
        /// </summary>
        /// <param name="surveyFilename">File with the survey results.</param>
        /// <param name="isFirstHalf">If true, this file is the survey after the first half of the experiment.</param>
        /// <param name="isGeneratedFirstGroup">If true, this file is the group which played the generated encounters first.</param>
        /// <param name="lineToFill">Output parameter. We will fill this CSV line with the summarization of the survey.</param>
        private void FillExperimentHalfSummary(string surveyFilename, bool isFirstHalf, bool isGeneratedFirstGroup, ExperimentSummaryCsvLine lineToFill)
        {
            var summary = new ExperimentHalfSummary();
            if (isFirstHalf == isGeneratedFirstGroup)
            {
                lineToFill.GeneratedLevelsSummary = summary;
            }
            else
            {
                lineToFill.StaticLevelsSummary = summary;
            }
            using (var surveyReader = new StreamReader(surveyFilename))
            {
                // Ignore header;
                surveyReader.ReadCsvLine();
                // Hardcoded indexes as usual, not pretty, but faster than creating a model for the file.
                var cells = surveyReader.ReadCsvLine();
                summary.Rating = int.Parse(cells[6]);
                var door1Rating = int.Parse(cells[7]);
                var door2Rating = int.Parse(cells[8]);
                var door3Rating = int.Parse(cells[9]);
                if (isFirstHalf)
                {
                    summary.MediumDoorRating = door1Rating;
                    summary.HardDoorRating = door2Rating;
                    summary.EasyDoorRating = door3Rating;
                }
                else
                {
                    summary.HardDoorRating = door1Rating;
                    summary.EasyDoorRating = door2Rating;
                    summary.MediumDoorRating = door3Rating;
                }
                double currentFlowSum = 0;
                for (int i = 10; i < 20; i++)
                {
                    currentFlowSum += int.Parse(cells[i]);
                }
                summary.FlowScore = currentFlowSum / 10;
                double difficultySum = 0;
                for (int i = 20; i < 23; ++i)
                {
                    difficultySum += int.Parse(cells[i]);
                }
                summary.PerceivedDifficultyScore = int.Parse(cells[22]);
            }
        }
        /// <summary>
        /// Fill the demographics information in the summary.
        /// </summary>
        /// <param name="surveyFilename">Filename of the survey with the demographic info.</param>
        /// <param name="lineToFill">Output parameter, the CSV line to fill with the experiment results.</param>
        private void FillDemographics(string surveyFilename, ExperimentSummaryCsvLine lineToFill)
        {
            var demographics = new ExperimentDemographics();
            lineToFill.Demographics = demographics;
            using (var surveyReader = new StreamReader(surveyFilename))
            {
                // Ignore header;
                surveyReader.ReadCsvLine();
                var cells = surveyReader.ReadCsvLine();
                demographics.Gender = cells[23];
                demographics.Age = cells[24];
                demographics.Education = cells[25];
                demographics.RpgsPlayed = cells[26].Split(';').Length - 1;
            }
        }
        /// <summary>
        /// Fills the info about experiment errors in the summary.
        /// </summary>
        /// <param name="fileName">File name of the file with the results of every encounter.</param>
        /// <param name="lineToFill">CSV line to fill with data.</param>
        private void FillCombatResultsSummary(string fileName, ExperimentSummaryCsvLine lineToFill)
        {
            // Retrieve all experiment errors.
            var rawData = new GeneralDataParser().LoadGeneralData(fileName, skipFirstNLines: 1);
            var errors = (from line in rawData
                                 where (line as CombatOverLine)?.WasLogged == true
                                 select Math.Abs((line as CombatOverLine).DifficultyError)).ToList();
            // Fill out all the averages.
            var halves = SplitIntoParts(errors, 2);
            var quarters = SplitIntoParts(errors, 4);
            lineToFill.AverageErrorAll = errors.Average();
            lineToFill.AverageErrorsHalves[0] = halves[0].Average();
            lineToFill.AverageErrorsHalves[1] = halves[1].Average();
            lineToFill.AverageErrorsQuarters[0] = quarters[0].Average();
            lineToFill.AverageErrorsQuarters[1] = quarters[1].Average();
            lineToFill.AverageErrorsQuarters[2] = quarters[2].Average();
            lineToFill.AverageErrorsQuarters[3] = quarters[3].Average();
        }
        /// <summary>
        /// Evenly split the list into several parts.
        /// </summary>
        /// <typeparam name="T">Type of the parameters in the collection.</typeparam>
        /// <param name="toSplit">The collection to split.</param>
        /// <param name="parts">Into how many parts should the collection be split.</param>
        /// <returns></returns>
        private List<List<T>> SplitIntoParts<T>(List<T> toSplit, int parts)
        {
            var toReturn = new List<List<T>>(parts);
            int currentArrayStart = 0;
            for (int i = 0; i < parts; ++i)
            {
                var partLength = toSplit.Count / parts;
                if (i < toSplit.Count % parts)
                {
                    partLength += 1;
                }
                toReturn.Add(new List<T>(partLength));
                for (var j = currentArrayStart; j < currentArrayStart + partLength; ++j)
                {
                    toReturn[i].Add(toSplit[j]);
                }
                currentArrayStart = currentArrayStart + partLength;
            }
            return toReturn;
        }
    }
}
