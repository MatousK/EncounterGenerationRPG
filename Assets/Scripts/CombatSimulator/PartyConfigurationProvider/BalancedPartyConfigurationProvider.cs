using Assets.Scripts.Combat;

namespace Assets.Scripts.CombatSimulator.PartyConfigurationProvider
{
    /// <summary>
    /// <inheritdoc/>
    /// This provider splits the power ups equally between the characters.
    /// </summary>
    public class BalancedPartyConfigurationProvider : PartyConfigurationProvider
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
                ClericStats = GetStats(HeroProfession.Cleric, healthPowerups: powerups / 3, attackPowerups: powerups / 3),
                KnightStats = GetStats(HeroProfession.Knight, healthPowerups: powerups / 3, attackPowerups: powerups / 3),
                RangerStats = GetStats(HeroProfession.Ranger, healthPowerups: powerups / 3, attackPowerups: powerups / 3),
            };
        }
        /// <summary>
        /// Returns the string representation of this class.
        /// </summary>
        /// <returns>The string representation of this class.</returns>
        public override string ToString()
        {
            return "Balanced provider tier " + TierIndex.ToString();
        }
    }
}