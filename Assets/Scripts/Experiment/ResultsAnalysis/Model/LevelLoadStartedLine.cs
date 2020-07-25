using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Experiment.ResultsAnalysis.Model
{
    /// <summary>
    /// <inheritdoc/>
    /// This line indicates that the level started loading.
    /// </summary>
    class LevelLoadStartedLine: CsvLine
    {
        /// <summary>
        /// Level being loaded.
        /// </summary>
        public int LevelIndex;
    }
}
