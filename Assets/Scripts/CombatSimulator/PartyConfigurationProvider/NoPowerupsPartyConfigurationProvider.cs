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
                ClericStats = GetStats(baseHp: ClericBaseHp, healthPowerups: 0, baseAttack: ClericBaseAttack, attackPowerups: 0),
                KnightStats = GetStats(baseHp: KnightBaseHp, healthPowerups: 0, baseAttack: KnightBaseAttack, attackPowerups: 0),
                RangerStats = GetStats(baseHp: RangerBaseHp, healthPowerups: 0, baseAttack: RangerBaseAttack, attackPowerups: 0),
            };
        }

        public override string ToString()
        {
            return "No powerups";
        }
    }
}
