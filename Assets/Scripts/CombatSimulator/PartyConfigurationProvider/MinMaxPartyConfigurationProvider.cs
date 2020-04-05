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
                ClericStats = GetStats(baseHp: ClericBaseHp, healthPowerups: 0, baseAttack: ClericBaseAttack, attackPowerups: 0),
                KnightStats = GetStats(baseHp: KnightBaseHp, healthPowerups: powerups, baseAttack: KnightBaseAttack, attackPowerups: 0),
                RangerStats = GetStats(baseHp: RangerBaseHp, healthPowerups: 0, baseAttack: RangerBaseAttack, attackPowerups: powerups),
            };
        }

        public override string ToString()
        {
            return "Min max provider tier " + TierIndex.ToString(); 
        }
    }
}
