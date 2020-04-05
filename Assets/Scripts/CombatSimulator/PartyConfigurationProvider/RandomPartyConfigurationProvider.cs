namespace Assets.Scripts.CombatSimulator.PartyConfigurationProvider
{
    public class RandomPartyConfigurationProvider : PartyConfigurationProvider
    {
        public int TierIndex;
        public int TierIncrement;
        public override PartyConfiguration GetPartyConfiguration()
        {
            int powerups = TierIndex * TierIncrement;
            var healthPowerupDistribution = SplitPowerupsIntoGroups(powerups);
            var danagePowerupDistribution = SplitPowerupsIntoGroups(powerups);
            return new PartyConfiguration
            {
                ClericStats = GetStats(baseHp: ClericBaseHp, healthPowerups: healthPowerupDistribution.ClericPowerups, baseAttack: ClericBaseAttack, attackPowerups: danagePowerupDistribution.ClericPowerups),
                KnightStats = GetStats(baseHp: KnightBaseHp, healthPowerups: healthPowerupDistribution.KnightPowerups, baseAttack: KnightBaseAttack, attackPowerups: danagePowerupDistribution.KnightPowerups),
                RangerStats = GetStats(baseHp: RangerBaseHp, healthPowerups: healthPowerupDistribution.RangerPowerups, baseAttack: RangerBaseAttack, attackPowerups: danagePowerupDistribution.RangerPowerups),
            };
        }

        private SplitResult SplitPowerupsIntoGroups(int powerupCount)
        {
            SplitResult toReturn = new SplitResult();
            for (int i = 0; i < powerupCount; ++i)
            {
                int randomResult = UnityEngine.Random.Range(0, 3);
                switch (randomResult)
                {
                    case 0:
                        toReturn.ClericPowerups++;
                        break;
                    case 1:
                        toReturn.RangerPowerups++;
                        break;
                    case 2:
                        toReturn.KnightPowerups++;
                        break;
                }
            }
            return toReturn;
        }

        struct SplitResult
        {
            public int ClericPowerups;
            public int RangerPowerups;
            public int KnightPowerups;
        }

        public override string ToString()
        {
            return "Random provider tier " + TierIndex.ToString();
        }
    }
}
