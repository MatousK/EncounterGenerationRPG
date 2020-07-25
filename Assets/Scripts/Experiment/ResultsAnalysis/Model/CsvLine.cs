using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Experiment.ResultsAnalysis.Model
{
    /// <summary>
    /// This objects represents a single CSV line that can appear in the results CSV line that was previously logged by the <see cref="Analytics.AnalyticsService"/>.
    /// </summary>
    public abstract class CsvLine
    {
        /// <summary>
        /// Raw string with the data about the line.
        /// </summary>
        public string RawLineData;
        /// <summary>
        /// ID of the user.
        /// </summary>
        public string UserId;
        /// <summary>
        /// Time when the test result was logged.
        /// </summary>
        public DateTime LogTime;
        /// <summary>
        /// Type of the line, i.e. what information the line holds.
        /// </summary>
        public string LineType;
        /// <summary>
        /// Which version of the experiment is this.
        /// </summary>
        public int Version;
    }
}
