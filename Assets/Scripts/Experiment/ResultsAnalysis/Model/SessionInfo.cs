using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Experiment.ResultsAnalysis.Model
{
    /// <summary>
    /// Information about a single experiment session.
    /// </summary>
    public class SessionInfo
    {
        /// <summary>
        /// Which experiment group did the player belong to.
        /// </summary>
        public SessionGroup SessionGroup;
        /// <summary>
        /// How many levels did the player clear.
        /// </summary>
        public int ClearedLevels;
    }
    /// <summary>
    /// An experiment group for the session.
    /// </summary>
    public enum SessionGroup
    {
        /// <summary>
        /// The player did not finish the tutorial.
        /// </summary>
        TutorialOnly,
        /// <summary>
        /// The player first played the generated encounter, then he played the static encounters.
        /// </summary>
        FirstGeneratedThenStatic,
        /// <summary>
        /// The player first played the static encounter, then he played the generated encounters.
        /// </summary>
        FirstStaticThenGenerated, 
        /// <summary>
        /// The player encountered the bug which caused the enemies to stop spawning and messed up the matrix.
        /// </summary>
        InvalidValues
    }
}
