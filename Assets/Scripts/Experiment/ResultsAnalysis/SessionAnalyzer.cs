using Assets.Scripts.EncounterGenerator.Model;
using Assets.Scripts.EncounterGenerator.Utils;
using Assets.Scripts.Experiment.ResultsAnalysis.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Experiment.ResultsAnalysis
{
    /// <summary>
    /// Analyzes a single session. Goes only through the CSV data from the analytics backend.
    /// Initializes output directory, stores the raw data there and does the matrix reconstruction if requested.
    /// Destroys itself once done.
    /// </summary>
    class SessionAnalyzer: MonoBehaviour
    {
        /// <summary>
        /// Paths for output and input file of the result analysis
        /// </summary>
        public ResultAnalysisConfiguration Configuration;
        /// <summary>
        /// If true, this analyzer should also reconstruct the matrix as it developed throughout the session and save the visualizations.
        /// </summary>
        public bool ShouldReconstructMatrix;
        /// <summary>
        /// The component responsible for reconstructing the matrix. Null if reconstruction is done or not required.
        /// </summary>
        MatrixReconstructionManager matrixReconstructionManager;
        /// <summary>
        /// Starts the analysis. Once done, the object destroys itself.
        /// </summary>
        /// <param name="lines">All lines in the current session which this class should analyze.</param>
        /// <param name="initialDifficultyMatrix">The matrix to be used as the initial matrix for the reconstruction.</param>
        public void AnalyzeSession(List<CsvLine> lines, EncounterDifficultyMatrix initialDifficultyMatrix)
        {
            var sessionGroup = GetGroup(lines);
            int levelsCompleted = lines.Count(line => line is LevelLoadStartedLine);
            var resultsFolder = GetSessionResultsFolder(sessionGroup, levelsCompleted, lines.First().UserId, lines.First().Version);
            Directory.CreateDirectory(resultsFolder);
            if (ShouldReconstructMatrix)
            {
                matrixReconstructionManager = gameObject.AddComponent<MatrixReconstructionManager>();
                matrixReconstructionManager.CombatOverLines = lines.Select(line => line as CombatOverLine).Where(line => line != null).ToList();
                matrixReconstructionManager.ResultsFolder = resultsFolder;
                matrixReconstructionManager.CurrentMatrix = initialDifficultyMatrix;
                matrixReconstructionManager.Version = lines.First().Version;
            }
            SaveAllSessionLines(lines, resultsFolder);
        }
        /// <summary>
        /// Called every frame, if we did not have to reconstruct the matrix or if it is done, destroy self.
        /// </summary>
        private void Update()
        {
            if (matrixReconstructionManager == null)
            {
                Destroy(gameObject);
            }
        }
        /// <summary>
        /// Get the output folder for the given parameters for this result analysis.
        /// </summary>
        /// <param name="group">Group to which the session belongs to.</param>
        /// <param name="levelsCompleted">How many levels were completed during the session.</param>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="version">The version of the experiment.</param>
        /// <returns>The folder where the output data for this session should be stored.</returns>
        private string GetSessionResultsFolder(SessionGroup group, int levelsCompleted, string userId, int version)
        {
            string groupHumanReadableName = "";
            switch (group)
            {
                case SessionGroup.FirstGeneratedThenStatic:
                    groupHumanReadableName = Configuration.GeneratedFirstGroupName;
                    break;
                case SessionGroup.FirstStaticThenGenerated:
                    groupHumanReadableName = Configuration.StaticFirstGroupName;
                    break;
                case SessionGroup.TutorialOnly:
                    groupHumanReadableName = Configuration.TutorialOnlyGroupName;
                    break;
                case SessionGroup.InvalidValues:
                    groupHumanReadableName = Configuration.InvalidValuesGroupName;
                    break;
            }
            return $"{Configuration.ResultsRootDirectory}/v{version}/{groupHumanReadableName}/{levelsCompleted}/{userId}/";
        }
        /// <summary>
        /// Determine the experiment group of the current session.
        /// </summary>
        /// <param name="lines">All lines in the experiment.</param>
        /// <returns>The experiment group this session belongs to.</returns>
        private SessionGroup GetGroup(List<CsvLine> lines)
        {
            // First, check for NaN, which are the sign of the bug that screwed up the matrix.
            if (lines.Any(line =>
            {
                var combatLine = line as CombatOverLine;
                return combatLine != null && (float.IsNaN(combatLine.ExpectedDifficulty) || float.IsInfinity(combatLine.ExpectedDifficulty));
            }))
            {
                return SessionGroup.InvalidValues;
            }
            int currentLineIndex = -1;
            // Find the first combat after tutorial. If it is generated, we are in group with generated encounters first.
            while (++currentLineIndex < lines.Count && !(lines[currentLineIndex] is LevelLoadStartedLine)) ;
            while (++currentLineIndex < lines.Count && !(lines[currentLineIndex] is CombatOverLine)) ;
            if (currentLineIndex >= lines.Count)
            {
                // We did not an end of combat after tutorial. So the user did not finish the tutorial.
                return SessionGroup.TutorialOnly;
            }
            else
            {
                return (lines[currentLineIndex] as CombatOverLine).WasStaticEncounter ? SessionGroup.FirstStaticThenGenerated : SessionGroup.FirstGeneratedThenStatic;
            }
        }
        /// <summary>
        /// Dumps all the lines in the output directory.
        /// </summary>
        /// <param name="lines">All the CSV lines for this session..</param>
        /// <param name="resultsFolder">Folder where we should dump those.</param>
        private void SaveAllSessionLines(List<CsvLine> lines, string resultsFolder)
        {
            var rawDataFilename = resultsFolder + Configuration.ProcessedRawDataFileName;
            using (StreamWriter sw = new StreamWriter(rawDataFilename))
            {
                sw.WriteLine("sep=;");
                foreach (var line in lines)
                {
                    sw.WriteLine(line.RawLineData);
                }
            }
        }
    }
}
