using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Combat;
using Assets.Scripts.CombatSimulator;
using Assets.Scripts.EncounterGenerator.Algorithm;
using Assets.Scripts.EncounterGenerator.Configuration;
using Assets.Scripts.EncounterGenerator.Model;
using UnityEngine;

namespace Assets.Scripts.EncounterGenerator.Utils
{
    /// <summary>
    /// When placed on the scene and we are playing a development or editor build, listen to the changes in the development matrix and logs the results..
    /// The component can also be used manually by repeatedly calling <see cref="LogMatrixChange(MatrixChangedEventArgs, EncounterDifficultyMatrix, bool)"/>
    /// This is a DontDestroyOnLoad component, will persist between scenes and destroy itself if another instance exists.
    /// </summary>
    public class EncounterGeneratorLogger: MonoBehaviour
    {
        /// <summary>
        /// The object providing the difficulty matrix information.
        /// </summary>
        private DifficultyMatrixProvider matrixProvider;
        /// <summary>
        /// Object which can generate visualizations of a difficulty matrix.
        /// </summary>
        private MatrixVisualizer matrixVisualizer;
        /// <summary>
        /// General configuration of the encounter generator.
        /// </summary>
        private EncounterGeneratorConfiguration configuration;
        /// <summary>
        /// The folder where we should store the log files and visualizations.
        /// </summary>
        public string ResultsDirectory;
        /// <summary>
        /// The index of the test being logged, incremented after ever result logged.
        /// </summary>
        private int currentTestIndex;

        private void Awake()
        {
            if (FindObjectsOfType<EncounterGeneratorLogger>().Length > 1)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
            matrixVisualizer = FindObjectOfType<MatrixVisualizer>();
            configuration = EncounterGeneratorConfiguration.CurrentConfig;
        }

#if DEVELOPMENT_BUILD || UNITY_EDITOR
        /// <summary>
        /// Called before the first update method. Called only in editor and development builds.
        /// Initializes result folders and starts listening to matrix changes.
        /// Will also save the initial matrix.
        /// </summary>
        private void Start()
        {
            matrixProvider = FindObjectsOfType<DifficultyMatrixProvider>().First(provider => !provider.IsPendingKill);
            matrixProvider.MatrixChanged += MatrixProvider_MatrixChanged;
            InitTestResultsDirectory();
            if (matrixProvider.CurrentDifficultyMatrix != null)
            {
                var visualizationFileName = $"{ResultsDirectory}Visualization.png";
                Task.Run(() => matrixVisualizer.SaveMatrix(visualizationFileName, matrixProvider.CurrentDifficultyMatrix));
            }
        }
#endif
        /// <summary>
        /// Logs the change in a log file and generates the visualization of the matrix.
        /// </summary>
        /// <param name="e">Information about the change in the matrix.</param>
        /// <param name="matrix">The difficulty matrix which was just updated.</param>
        /// <param name="async">If true, the update should be done on a separate thread as much as possible.</param>
        public void LogMatrixChange(MatrixChangedEventArgs e, EncounterDifficultyMatrix matrix, bool async)
        {
            LogResult(e);
            var visualizationFileName = $"{ResultsDirectory}Visualization{currentTestIndex}.png";
            // This will probably take a long time, run on a different thread.
            if (async)
            {
                Task.Run(() => matrixVisualizer.SaveMatrix(visualizationFileName, matrix));
            } 
            else
            {
                matrixVisualizer.SaveMatrix(visualizationFileName, matrix);
            }
            currentTestIndex++;
        }
        /// <summary>
        /// Called when the matrix is updated. Logs the results.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Information about the change in the matrix.</param>
        private void MatrixProvider_MatrixChanged(object sender, MatrixChangedEventArgs e)
        {
            LogMatrixChange(e, matrixProvider.CurrentDifficultyMatrix, true);
        }
        /// <summary>
        /// Log the result of a combat and matrix change into a log file.
        /// </summary>
        /// <param name="e">Information about the change in the matrix.</param>
        private void LogResult(MatrixChangedEventArgs e)
        {
            var resultsFilename = $"{ResultsDirectory}log.txt";
            var partyStrength = e.PartyAttack[HeroProfession.Ranger] * e.PartyHitpoints[HeroProfession.Ranger] +
                                e.PartyAttack[HeroProfession.Knight] * e.PartyHitpoints[HeroProfession.Knight] +
                                e.PartyAttack[HeroProfession.Cleric] * e.PartyHitpoints[HeroProfession.Cleric];
            using (var outputStream = new StreamWriter(resultsFilename, true))
            {
                outputStream.WriteLine("************************************************************************");
                outputStream.WriteLine();
                outputStream.WriteLine($"Test results no.{currentTestIndex}");
                outputStream.WriteLine($"Party Strength:{partyStrength}");
                outputStream.WriteLine($"Monsters Strength:{e.FoughtEncounter.GetAdjustedMonsterCount(configuration)}");
                outputStream.WriteLine($"Expected difficulty:{e.DifficultyEstimate}");
                outputStream.WriteLine($"Real difficulty:{e.DifficultyReality}");
                outputStream.WriteLine($"Error in difficulty:{e.DifficultyEstimate - e.DifficultyReality }");
                if (e.WasGameOver)
                {
                    outputStream.WriteLine();
                    outputStream.WriteLine("!!!!!!PARTY WIPE!!!!!!");
                    outputStream.WriteLine();
                }
                outputStream.WriteLine("Party stats:");
                outputStream.WriteLine($"Ranger - Attack: {e.PartyAttack[HeroProfession.Ranger]}, HP: {e.PartyHitpoints[HeroProfession.Ranger]}");
                outputStream.WriteLine($"Knight - Attack: {e.PartyAttack[HeroProfession.Knight]}, HP: {e.PartyHitpoints[HeroProfession.Knight]}");
                outputStream.WriteLine($"Cleric - Attack: {e.PartyAttack[HeroProfession.Cleric]}, HP: {e.PartyHitpoints[HeroProfession.Cleric]}");
                outputStream.WriteLine("Monsters:");
                foreach (var monsterGroup in e.FoughtEncounter.AllEncounterGroups)
                {
                    if (monsterGroup.MonsterCount == 0)
                    {
                        continue;
                    }
                    outputStream.WriteLine($"{monsterGroup.MonsterCount}x {monsterGroup.MonsterType.Rank}  {monsterGroup.MonsterType.Role}");
                }
                outputStream.WriteLine();
                outputStream.WriteLine("************************************************************************");
            }
        }
        /// <summary>
        /// Creates the output directory if it is not set yet.
        /// Used for automatic matrix saves, directory is based on current time.
        /// </summary>
        private void InitTestResultsDirectory()
        {
            if (string.IsNullOrEmpty(ResultsDirectory))
            {
                ResultsDirectory = $"TestResults/{DateTime.Now:yy-MM-dd-HH-mm-ss}/";
                Directory.CreateDirectory(ResultsDirectory);
            }
        }
        /// <summary>
        /// When destroyed, unsubscribe from the matrix changed event.
        /// </summary>
        private void OnDestroy()
        {
            if (matrixProvider != null)
            {
                matrixProvider.MatrixChanged -= MatrixProvider_MatrixChanged;
            }
        }

    }
}
