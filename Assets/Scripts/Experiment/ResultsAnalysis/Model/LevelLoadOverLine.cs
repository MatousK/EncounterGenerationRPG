using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Experiment.ResultsAnalysis.Model
{
    /// <summary>
    /// <inheritdoc/>
    /// This line represents a level being successfully loaded.
    /// </summary>
    class LevelLoadOverLine: CsvLine
    {
        /// <summary>
        /// Index of the level that was loaded.
        /// </summary>
        public int LevelIndex;
    }
}
