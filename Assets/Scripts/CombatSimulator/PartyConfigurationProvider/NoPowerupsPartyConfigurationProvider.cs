using Assets.Scripts.Combat;

namespace Assets.Scripts.CombatSimulator.PartyConfigurationProvider
{
    /// <summary>
    /// This class ignores powerups and returns a configuration with no powerups.
    /// </summary>
    public class NoPowerupsPartyConfigurationProvider : global::Assets.Scripts.CombatSimulator.PartyConfigurationProvider.PartyConfigurationProvider
    {
        public override PartyConfiguration GetPartyConfiguration()
        {
            return new PartyConfiguration
            {
                ClericStats = GetStats(HeroProfession.Cleric, healthPowerups: 0, attackPowerups: 0),
                KnightStats = GetStats(HeroProfession.Knight, healthPowerups: 0, attackPowerups: 0),
                RangerStats = GetStats(HeroProfession.Ranger, healthPowerups: 0, attackPowerups: 0),
            };
        }

        public override string ToString()
        {
            return "No powerups";
        }
    }
}
