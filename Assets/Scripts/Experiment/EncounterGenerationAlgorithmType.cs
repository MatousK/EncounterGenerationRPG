using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Experiment
{
    /// <summary>
    /// Specifies a method of generating encounters.
    /// </summary>
    public enum EncounterGenerationAlgorithmType
    {
        /// <summary>
        /// Encounters are created statically by the designer.
        /// </summary>
        StaticGenerator,
        /// <summary>
        /// Encounters are generated at runtime by the encounter generation algorithm.
        /// </summary>
        MatrixBasedGenerator
    }
}
