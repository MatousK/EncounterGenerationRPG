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
    class ExperimentSummarizationHelper
    {
        public ExperimentSummarizationHelper(ResultAnalysisConfiguration configuration)
        {
            this.configuration = configuration;
        }

        ResultAnalysisConfiguration configuration;

        public void SummarizeExperiment()
        {
            var summaryDirectory = Path.GetDirectoryName(configuration.SummaryFilePath);
            Directory.CreateDirectory(summaryDirectory);
            using (var outputWriter = new StreamWriter(configuration.SummaryFilePath))
            {
                outputWriter.WriteLine("sep=;");
                ExperimentSummaryCsvLine.WriteHeader(outputWriter);
                var resultsRoot = configuration.ResultsRootDirectory;
                var versionFolders = configuration.AllowedVersions.Select(version => resultsRoot + version);
                foreach (var versionFolder in versionFolders)
                {
                    SummarizeVersionFolder(versionFolder, outputWriter);
                }
            }
        }

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
                var levelsFinishedFolders = configuration.AllowedLevelsFinished.Select(version => directory + "/" + version);
                foreach (var levelsFinishedFolder in levelsFinishedFolders)
                {
                    SummarizeLevelsFinishedFolder(groupName, levelsFinishedFolder, outputWriter);
                }
            }
        }

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

        private void SummarizeSingleTest(string groupName, string userId, string folder, StreamWriter outputWriter)
        {
            var summaryLine = new ExperimentSummaryCsvLine();
            summaryLine.Group = groupName;
            summaryLine.UserId = userId;
            bool isGeneratedFirstGroup = groupName == configuration.GeneratedFirstGroupName;
            var firstHalfFileName = folder + "/" + configuration.ProcessedSurveyHalfFileName;
            FillExperimentHalfSummary(firstHalfFileName, true, isGeneratedFirstGroup, summaryLine);
            var completeFileName = folder + "/" + configuration.ProcessedSurveyCompleteFileName;
            FillExperimentHalfSummary(completeFileName, false, isGeneratedFirstGroup, summaryLine);
            FillDemographics(completeFileName, summaryLine);
            summaryLine.WriteLine(outputWriter);
        }

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
                summary.PerceivedDifficultyScore = difficultySum / 3;
            }
        }

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
    }
}
