using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.EncounterGenerator.Algorithm;
using Assets.Scripts.EncounterGenerator.Configuration;
using Assets.Scripts.EncounterGenerator.Model;
using UnityEngine;

namespace Assets.Scripts.EncounterGenerator
{
    /// <summary>
    /// When this component is loaded, it loads the matrix data.
    /// It is then used to give the rest of the game objects reference to the matrix data.
    /// This is a DontDestroyOnLoad object. There should only ever be one instance of this component and it should persist between scenes.
    /// </summary>
    public class DifficultyMatrixProvider : MonoBehaviour
    {
        /// <summary>
        /// The currently loaded difficulty matrix that should be used by the encounter generator.
        /// </summary>
        public EncounterDifficultyMatrix CurrentDifficultyMatrix { get; private set; }
        /// <summary>
        /// This is mainly for testing - if we are in the main menu, we can do all the loading and precomputations on a separate thread, we have plenty of time.
        /// If it is not in the menu, we must load it immediately. That should happen mostly when debugging a scene directly.
        /// </summary>
        public bool IsInMainMenu;
        /// <summary>
        /// If true, this object is about to be destroyed.
        /// We use this because encounter manager is initializing in the same frame as the matrix provider.
        /// Without checking IsPendingKill, it would find the new instance.
        /// </summary>
        public bool IsPendingKill;
        /// <summary>
        /// This event is raised whenever a matrix is updated after an encounter.
        /// </summary>
        public event EventHandler<MatrixChangedEventArgs> MatrixChanged;

        private void Awake()
        {
            if (FindObjectsOfType<DifficultyMatrixProvider>().Length > 1)
            {
                IsPendingKill = true;
                Destroy(gameObject, 0);
                return;
            }
            DontDestroyOnLoad(this);
            ReloadMatrix(IsInMainMenu);
        }
        /// <summary>
        /// Loads the matrix from the source file, replacing the old one if some matrix was already stored in <see cref="CurrentDifficultyMatrix"/>
        /// Is called when the game is loaded and after an experiment is finished (to reset the matrix for the next experiment).
        /// </summary>
        /// <param name="async"></param>
        public void ReloadMatrix(bool async)
        {
            var matrixString = Resources.Load<TextAsset>("Matrix").text;
            UnityEngine.Debug.Log($"Matrix loaded. Characters: {matrixString.Length}");
            if (async)
            {
                UnityEngine.Debug.Log("Matrix will be loaded asynchronously.");
                // Run on a different thread so we do not blockUI while in main menu.
                Task.Run(() => LoadMatrix(matrixString));
            }
            else
            {
                UnityEngine.Debug.Log("Matrix will be loaded synchronously.");
                LoadMatrix(matrixString);
            }
        }
        /// <summary>
        /// Called when the matrix is changed. Propagates the event to the <see cref="MatrixChanged"/> event.
        /// We need this because the matrix object might change and be reloaded. And e.g. the matrix visualizer is registering only once to matrix changed, so it must register to this provider
        /// instead to the matrix directly.
        /// </summary>
        /// <param name="e">Information about the change in the matrix.</param>
        public void OnMatrixChanged(MatrixChangedEventArgs e)
        {
            MatrixChanged?.Invoke(this, e);
        }
        /// <summary>
        /// Loads the entire matrix from the specified string. Can be run on another thread.
        /// </summary>
        /// <param name="matrixString">String representation of the matrix.</param>
        private void LoadMatrix(string matrixString)
        {
            try
            {
                UnityEngine.Debug.Log("Started loading matrix.");
                var config = EncounterGeneratorConfiguration.CurrentConfig;
                UnityEngine.Debug.Log("Config initialized.");
                using (var sr = new StringReader(matrixString))
                {
                    UnityEngine.Debug.Log("Stream opened.");
                    var matrixSource = DifficultyMatrixParser.ParseFile(sr);
                    UnityEngine.Debug.Log("Matrix parsed successfully.");
                    CurrentDifficultyMatrix = new EncounterDifficultyMatrix();
                    UnityEngine.Debug.Log("Matrix object created.");
                    foreach (var sourceLine in matrixSource)
                    {
                        var newMatrixRow = new EncounterDifficultyMatrixElement(sourceLine);
                        newMatrixRow.EncounterGroups.UpdatePrecomputedMonsterCount(config);
                        CurrentDifficultyMatrix.MatrixElements.Add(newMatrixRow);
                    }
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e);
            }
        }
    }
}
