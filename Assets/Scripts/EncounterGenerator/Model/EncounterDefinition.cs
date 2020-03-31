using EncounterGenerator.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncounterGenerator.Model
{
    /// <summary>
    /// Represents all monsters that might spawn in an encounter.
    /// </summary>
    public struct EncounterDefinition
    {
        /// <summary>
        /// All of the monsters that should spawn in the 
        /// </summary>
        public List<MonsterGroup> AllEncounterGroups;

        public float GetAdjustedMonsterCount(EncounterGeneratorConfiguration configuration)
        {
            return AllEncounterGroups.Sum(group => group.GetAdjustedMonsterCount(configuration));
        }

        public float GetDistance(EncounterDefinition other, EncounterGeneratorConfiguration configuration)
        {
            // TODO: Come up with a better distance algorithm.
            return Math.Abs(other.GetAdjustedMonsterCount(configuration) - GetAdjustedMonsterCount(configuration));
        }
    }
}