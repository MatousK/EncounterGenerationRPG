using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.EncounterGenerator.Configuration;
using Assets.Scripts.EncounterGenerator.Model;
using UnityEngine;

namespace Assets.Scripts.EncounterGenerator
{
    class DifficultyMatrixProvider : MonoBehaviour
    {
        public EncounterDifficultyMatrix CurrentDifficultyMatrix { get; private set; }

        private void Awake()
        {
            if (FindObjectsOfType<DifficultyMatrixProvider>().Length > 1)
            {
                Destroy(this);
                return;
            }
            DontDestroyOnLoad(this);
            LoadMatrix();
        }

        private void LoadMatrix()
        {
            // TODO: Load from some shared storage, make it a singleton, something, this is ugly.
            var config = new EncounterGeneratorConfiguration();
            // TODO: Load asynchronously
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
