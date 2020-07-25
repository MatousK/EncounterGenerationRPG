using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Experiment.ResultsAnalysis
{
    /// <summary>
    /// <inheritdoc/>
    /// It tries the folders by the ID of the user. This should be always accurate.
    /// </summary>
    class SurveyWithIdAnalyzer : SurveyAnalyzerBase
    {
        /// <summary>
        /// Initialize a new instance of the <see cref="SurveyWithIdAnalyzer"/> class.
        /// </summary>
        /// <param name="configuration"><inheritdoc/></param>
        public SurveyWithIdAnalyzer(ResultAnalysisConfiguration configuration) : base(configuration)
        {

        }
        /// <summary>
        /// <inheritdoc/>
        /// Tries to find the directory whose name is the same as the ID of the CSV line.
        /// </summary>
        /// <param name="cells"><inheritdoc/></param>
        /// <param name="allDirectories"><inheritdoc/></param>
        /// <returns><inheritdoc/></returns>
        protected override string GetTargetDirectory(List<string> cells, string[] allDirectories)
        {
            // Ugly, hardcoded cell index.
            var id = cells[5];
            foreach (var directory in allDirectories)
            {
                var directoryName = Path.GetFileName(directory);
                if (directoryName == id)
                {
                    return directory;
                }
            }
            UnityEngine.Debug.LogError($"No result found for id {id}");
            return null;
        }
    }
}
