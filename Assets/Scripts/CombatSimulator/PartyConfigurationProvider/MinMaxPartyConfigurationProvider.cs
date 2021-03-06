﻿using Assets.Scripts.Combat;

namespace Assets.Scripts.CombatSimulator.PartyConfigurationProvider
{
    /// <summary>
    /// <inheritdoc/>
    /// This strategy gives all health power ups to the knight and all damage power ups to the ranger.
    /// </summary>
    public class MinMaxPartyConfigurationProvider : global::Assets.Scripts.CombatSimulator.PartyConfigurationProvider.PartyConfigurationProvider
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
                ClericStats = GetStats(HeroProfession.Cleric, healthPowerups: 0, attackPowerups: 0),
                KnightStats = GetStats(HeroProfession.Knight, healthPowerups: powerups, attackPowerups: 0),
                RangerStats = GetStats(HeroProfession.Ranger, healthPowerups: 0,attackPowerups: powerups),
            };
        }

        /// <summary>
        /// Returns the string representation of this class.
        /// </summary>
        /// <returns>The string representation of this class.</returns>
        public override string ToString()
        {
            return "Min max provider tier " + TierIndex.ToString(); 
        }
    }
}
