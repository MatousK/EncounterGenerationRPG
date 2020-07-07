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
    public class MatrixVisualizer: MonoBehaviour
    {
        public Gradient VisualizationGradient;
        public float PartyPowerScale = 100;
        public float MonsterPowerScale = 100;
        public int VisualizationWidth = 1000;
        public int VisualizationHeight = 1000;
        private EncounterGeneratorConfiguration configuration = EncounterGeneratorConfiguration.CurrentConfig;
        private Dictionary<Vector2Int, List<float>> matrixDataPendingSave;
        private string pendingSavePath;

        private void Update()
        {
            if (matrixDataPendingSave != null && pendingSavePath != null)
            {
                SaveMatrixMainThread();
                matrixDataPendingSave = null;
                pendingSavePath = null;
            }
        }
        
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
