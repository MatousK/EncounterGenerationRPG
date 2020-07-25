using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Experiment
{
    /// <summary>
    /// Specifies the experiment group that can be assigned to the player.
    /// </summary>
    public enum ExperimentGroup
    {
        /// <summary>
        /// This player should play first tutorial level, then static levels, then dynamic levels.
        /// </summary>
        FirstStaticThenGenerated,
        /// <summary>
        /// This player should play first tutorial level, then dynamic levels, then statc levels.
        /// </summary>
        FirstGeneratedThenStatic
    }
}
