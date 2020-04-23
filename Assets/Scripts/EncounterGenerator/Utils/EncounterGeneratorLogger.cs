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
using UnityEngine;

namespace Assets.Scripts.EncounterGenerator.Utils
{
    public class EncounterGeneratorLogger: MonoBehaviour
    {
        private DifficultyMatrixProvider matrixProvider;
        private MatrixVisualizer matrixVisualizer;
        private EncounterGeneratorConfiguration configuration;
        private string resultsDirectory;
        private int currentTestIndex;

        public void Awake()
        {
            if (FindObjectsOfType<EncounterGeneratorLogger>().Length > 1)
            {
                Destroy(gameObject);
            }
            DontDestroyOnLoad(gameObject);
            matrixVisualizer = GetComponent<MatrixVisualizer>();
            // TODO: Get config from some common place;
            configuration = new EncounterGeneratorConfiguration();
        }

        private void Start()
        {
            matrixProvider = FindObjectsOfType<DifficultyMatrixProvider>().First(provider => !provider.IsPendingKill);
            matrixProvider.MatrixChanged += MatrixProvider_MatrixChanged;
            InitTestResultsDirectory();
            if (matrixProvider.CurrentDifficultyMatrix != null)
            {
                var visualizationFileName = $"{resultsDirectory}Visualization.bmp";
                Task.Run(() => matrixVisualizer.SaveMatrix(visualizationFileName, matrixProvider.CurrentDifficultyMatrix));
            }
        }

        private void MatrixProvider_MatrixChanged(object sender, MatrixChangedEventArgs e)
        {
            LogResult(e);
            var visualizationFileName = $"{resultsDirectory}Visualization{currentTestIndex}.bmp";
            // This will probably take a long time, run on a different thread.
            Task.Run(() => matrixVisualizer.SaveMatrix(visualizationFileName, matrixProvider.CurrentDifficultyMatrix));
            currentTestIndex++;
        }

        public void LogResult(MatrixChangedEventArgs e)
        {
            var resultsFilename = $"{resultsDirectory}log.txt";
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

        private void InitTestResultsDirectory()
        {
            resultsDirectory = $"TestResults/{DateTime.Now:yy-MM-dd-HH-mm-ss}/";
            Directory.CreateDirectory(resultsDirectory);
        }

        private void OnDestroy()
        {
            if (matrixProvider != null)
            {
                matrixProvider.MatrixChanged -= MatrixProvider_MatrixChanged;
            }
        }
    }
}
