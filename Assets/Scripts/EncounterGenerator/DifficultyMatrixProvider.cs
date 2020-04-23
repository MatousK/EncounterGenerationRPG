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
    class DifficultyMatrixProvider : MonoBehaviour
    {
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
            if (IsInMainMenu)
            {
                // Run on a different thread so we do not blockUI while in main menu.
                Task.Run(LoadMatrix);
            }
            else
            {
                LoadMatrix();
            }
        }

        public void OnMatrixChanged(MatrixChangedEventArgs e)
        {
            MatrixChanged?.Invoke(this, e);
        }

        private void LoadMatrix()
        {
            // TODO: Load from some shared storage, make it a singleton, something, this is ugly.
            var config = new EncounterGeneratorConfiguration();
            using (StreamReader sr = new StreamReader("Matrix.dat"))
            {
                var matrixSource = DifficultyMatrixParser.ParseFile(sr);
                CurrentDifficultyMatrix = new EncounterDifficultyMatrix();
                foreach (var sourceLine in matrixSource)
                {
                    var newMatrixRow = new EncounterDifficultyMatrixElement(sourceLine);
                    newMatrixRow.EncounterGroups.UpdatePrecomputedMonsterCount(config);
                    CurrentDifficultyMatrix.MatrixElements.Add(newMatrixRow);
                }
            }
        }
    }
}
