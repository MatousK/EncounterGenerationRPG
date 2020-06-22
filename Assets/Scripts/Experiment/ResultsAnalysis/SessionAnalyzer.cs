using Assets.Scripts.EncounterGenerator.Model;
using Assets.Scripts.EncounterGenerator.Utils;
using Assets.Scripts.Experiment.ResultsAnalysis.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Experiment.ResultsAnalysis
{
    class SessionAnalyzer: MonoBehaviour
    {
        MatrixReconstructionManager matrixReconstructionManager;
        const string ResultsRootDirectory = "Results/Processed/IndividualTests/";
        public void AnalyzeSession(List<CsvLine> lines, EncounterDifficultyMatrix initialDifficultyMatrix)
        {
            var sessionGroup = GetGroup(lines);
            int levelsCompleted = lines.Count(line => line is LevelLoadStartedLine);
            var resultsFolder = GetSessionResultsFolder(sessionGroup, levelsCompleted, lines.First().UserId, lines.First().Version);
            System.IO.Directory.CreateDirectory(resultsFolder);
            matrixReconstructionManager = gameObject.AddComponent<MatrixReconstructionManager>();
            matrixReconstructionManager.CombatOverLines = lines.Select(line => line as CombatOverLine).Where(line => line != null).ToList();
            matrixReconstructionManager.ResultsFolder = resultsFolder;
            matrixReconstructionManager.CurrentMatrix = initialDifficultyMatrix;
            matrixReconstructionManager.Version = lines.First().Version;
        }

        private void Update()
        {
            if (matrixReconstructionManager == null)
            {
                Destroy(this.gameObject);
            }
        }

        private string GetSessionResultsFolder(SessionGroup group, int levelsCompleted, string userId, int version)
        {
            string groupHumanReadableName = "";
            switch (group)
            {
                case SessionGroup.FirstGeneratedThenStatic:
                    groupHumanReadableName = "GeneratedFirst";
                    break;
                case SessionGroup.FirstStaticThenGenerated:
                    groupHumanReadableName = "StaticFirst";
                    break;
                case SessionGroup.TutorialOnly:
                    groupHumanReadableName = "TutorialOnly";
                    break;
                case SessionGroup.InvalidValues:
                    groupHumanReadableName = "InvalidValues";
                    break;
            }
            return $"{ResultsRootDirectory}/v{version}/{groupHumanReadableName}/{levelsCompleted}/{userId}/";
        }

        private SessionGroup GetGroup(List<CsvLine> lines)
        {
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
                return SessionGroup.TutorialOnly;
            }
            else
            {
                return (lines[currentLineIndex] as CombatOverLine).WasStaticEncounter ? SessionGroup.FirstStaticThenGenerated : SessionGroup.FirstGeneratedThenStatic;
            }
        }
    }
}
