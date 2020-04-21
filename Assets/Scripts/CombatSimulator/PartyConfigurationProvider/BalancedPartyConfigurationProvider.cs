using Assets.Scripts.Combat;

namespace Assets.Scripts.CombatSimulator.PartyConfigurationProvider
{
    public class BalancedPartyConfigurationProvider : global::Assets.Scripts.CombatSimulator.PartyConfigurationProvider.PartyConfigurationProvider
    {
        public int TierIndex;
        public int TierIncrement;
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

        public override string ToString()
        {
            return "Balanced provider tier " + TierIndex.ToString();
        }
    }
}