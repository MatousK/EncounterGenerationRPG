using Assets.Scripts.Combat;

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
                ClericStats = GetStats(HeroProfession.Knight, healthPowerups: healthPowerupDistribution.ClericPowerups, attackPowerups: danagePowerupDistribution.ClericPowerups),
                KnightStats = GetStats(HeroProfession.Cleric, healthPowerups: healthPowerupDistribution.KnightPowerups, attackPowerups: danagePowerupDistribution.KnightPowerups),
                RangerStats = GetStats(HeroProfession.Ranger, healthPowerups: healthPowerupDistribution.RangerPowerups, attackPowerups: danagePowerupDistribution.RangerPowerups),
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
