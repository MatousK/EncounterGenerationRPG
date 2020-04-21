using Assets.Scripts.Combat;

namespace Assets.Scripts.CombatSimulator.PartyConfigurationProvider
{
    public class MinMaxPartyConfigurationProvider : global::Assets.Scripts.CombatSimulator.PartyConfigurationProvider.PartyConfigurationProvider
    {
        public int TierIndex;
        public int TierIncrement;
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

        public override string ToString()
        {
            return "Min max provider tier " + TierIndex.ToString(); 
        }
    }
}
