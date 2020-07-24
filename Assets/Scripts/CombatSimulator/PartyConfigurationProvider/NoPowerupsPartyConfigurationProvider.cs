using Assets.Scripts.Combat;

namespace Assets.Scripts.CombatSimulator.PartyConfigurationProvider
{
    /// <summary>
    /// <inheritdoc/>
    /// This class ignores powerups and returns a configuration with no powerups.
    /// </summary>
    public class NoPowerupsPartyConfigurationProvider : global::Assets.Scripts.CombatSimulator.PartyConfigurationProvider.PartyConfigurationProvider
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns>The party configuration represented by this provider.</returns>
        public override PartyConfiguration GetPartyConfiguration()
        {
            return new PartyConfiguration
            {
                ClericStats = GetStats(HeroProfession.Cleric, healthPowerups: 0, attackPowerups: 0),
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
            return "No powerups";
        }
    }
}
