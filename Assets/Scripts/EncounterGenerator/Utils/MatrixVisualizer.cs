using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.EncounterGenerator.Configuration;
using Assets.Scripts.EncounterGenerator.Model;
using UnityEngine;

namespace Assets.Scripts.EncounterGenerator.Utils
{
    /// <summary>
    /// This class can create a graphic representation of the current matrix.
    /// Should only be used in development, as it is not optimized and freezes the game while generating the visualization.
    /// </summary>
    public class MatrixVisualizer: MonoBehaviour
    {
        /// <summary>
        /// The gradient which specifies what colors should different difficulty values have in the matrix.
        /// </summary>
        public Gradient VisualizationGradient;
        /// <summary>
        /// Party power divided by this is the x coordinate in the output image a matrix element represents.
        /// </summary>
        public float PartyPowerScale = 100;
        /// <summary>
        /// Monster power divided by this is the y coordinate in the output image a matrix element represents.
        /// </summary>
        public float MonsterPowerScale = 100;
        /// <summary>
        /// The width of the output file in pixels.
        /// </summary>
        public int VisualizationWidth = 1000;
        /// <summary>
        /// The height of the output file in pixels.
        /// </summary>
        public int VisualizationHeight = 1000;
        /// <summary>
        /// The general configuration of the encounter generation algorithm.
        /// </summary>
        private EncounterGeneratorConfiguration configuration = EncounterGeneratorConfiguration.CurrentConfig;
        /// <summary>
        /// First we process the data from the matrix and set for each pixel the difficulties of the elements that should affect it.
        /// This can be done on another thread.
        /// However, drawing to the texture itself uses Unity methods. So does the gradient evaluation. These can be only done on the main thread.
        /// So we save these preprocessed data and in the next update save them to the file.
        /// </summary>
        private Dictionary<Vector2Int, List<float>> matrixDataPendingSave;
        /// <summary>
        /// File path where the matrix from <see cref="matrixDataPendingSave"/> should be saved.
        /// </summary>
        private string pendingSavePath;
        /// <summary>
        /// Called every frame. If we have matrix data which have not been saved, save them. See <see cref="matrixDataPendingSave"/>
        /// </summary>
        private void Update()
        {
            if (matrixDataPendingSave != null && pendingSavePath != null)
            {
                SaveMatrixMainThread();
                matrixDataPendingSave = null;
                pendingSavePath = null;
            }
        }
        /// <summary>
        /// Generates the visualization for <paramref name="matrix"/> and saves it to <paramref name="path"/>.
        /// </summary>
        /// <param name="path">Path where the visualization should be saved.</param>
        /// <param name="matrix">The martix that should be saved.</param>
        public void SaveMatrix(string path, EncounterDifficultyMatrix matrix)
        {
            // This must be incredibly slow... But we are running on a different thread, so probably no big deal.
            Dictionary<Vector2Int, List<float>> coordinatesToDifficulty = new Dictionary<Vector2Int, List<float>>();
            foreach (var matrixElement in matrix.MatrixElements)
            {
                int partyPowerBucket = (int) (matrixElement.PartyPower / PartyPowerScale);
                int monsterPowerBucket = (int)(matrixElement.EncounterGroups.GetAdjustedMonsterCount(configuration) / MonsterPowerScale);
                var coordinates = new Vector2Int(partyPowerBucket, monsterPowerBucket);
                if (coordinatesToDifficulty.ContainsKey(coordinates))
                {
                    coordinatesToDifficulty[coordinates].Add(matrixElement.ResourcesLost);
                }
                else
                {
                    coordinatesToDifficulty[coordinates] = new List<float> { matrixElement.ResourcesLost };
                }
            }

            pendingSavePath = path;
            matrixDataPendingSave = coordinatesToDifficulty;
        }
        /// <summary>
        /// Saves the matrix data stored in <see cref="matrixDataPendingSave"/> to <see cref="pendingSavePath"/>.
        /// </summary>
        private void SaveMatrixMainThread()
        {
            // We divided matrix into buckets, now lets visualize it.
            Texture2D matrixVisualization = new Texture2D(VisualizationWidth, VisualizationHeight, TextureFormat.RGB24, false);
            for (int x = 0; x < VisualizationWidth; x++)
            {
                for (int y = 0; y < VisualizationHeight; y++)
                {
                    var coordinates = new Vector2Int(x, y);
                    if (!matrixDataPendingSave.ContainsKey(coordinates))
                    {
                        matrixVisualization.SetPixel(x, y, Color.white);
                        continue;
                    }
                    var averageDifficulty = matrixDataPendingSave[coordinates].Average();
                    if (float.IsNaN(averageDifficulty) || float.IsInfinity(averageDifficulty))
                    {
                        continue;
                    }
                    var pointColor = VisualizationGradient.Evaluate(averageDifficulty / 3);
                    matrixVisualization.SetPixel(x, y, pointColor); 
                }
            }
            matrixVisualization.Apply();
            var pngBytes = matrixVisualization.EncodeToPNG();
            File.WriteAllBytes(pendingSavePath, pngBytes);
        }
    }
}
