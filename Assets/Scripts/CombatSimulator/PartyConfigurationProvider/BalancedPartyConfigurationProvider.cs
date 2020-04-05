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
                ClericStats = GetStats(baseHp: ClericBaseHp, healthPowerups: powerups / 3, baseAttack: ClericBaseAttack, attackPowerups: powerups / 3),
                KnightStats = GetStats(baseHp: KnightBaseHp, healthPowerups: powerups / 3, baseAttack: KnightBaseAttack, attackPowerups: powerups / 3),
                RangerStats = GetStats(baseHp: RangerBaseHp, healthPowerups: powerups / 3, baseAttack: RangerBaseAttack, attackPowerups: powerups / 3),
            };
        }

        public override string ToString()
        {
            return "Balanced provider tier " + TierIndex.ToString();
        }
    }
}