using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Combat;

namespace Assets.Scripts.CombatSimulator.PartyConfigurationProvider
{
    /// <summary>
    /// <inheritdoc/>
    /// This provider gives all powerups to the cleric.
    /// </summary>
    class ClericPowerupsOnlyPartyConfigurationProvider : PartyConfigurationProvider
    {
        /// <summary>
        /// What is the current tier of power ups. Total number of power ups is <see cref="TierIndex"/> * <see cref="TierIncrement"/>
        /// </summary>
        public int TierIndex;
        /// <summary>
        /// How many power ups were picked up per tier. Total number of power ups is <see cref="TierIndex"/> * <see cref="TierIncrement"/>
        /// </summary>
        public int TierIncrement;
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns>The new party configuration.</returns>
        public override PartyConfiguration GetPartyConfiguration()
        {
            int powerups = TierIndex * TierIncrement;
            return new PartyConfiguration
            {
                ClericStats = GetStats(HeroProfession.Cleric, healthPowerups: powerups, attackPowerups: powerups),
                KnightStats = GetStats(HeroProfession.Knight, healthPowerups: 0, attackPowerups: 0),
                RangerStats = GetStats(HeroProfession.Ranger, healthPowerups: 0, attackPowerups: 0),
            };
        }

        /// <summary>
        /// Returns the string representation of this class.
        /// </summary>
        /// <returns>The string representation of this class.</returns>
        public override string ToString()
        {
            return "Cleric Powerups only " + TierIndex.ToString();
        }
    }
}
