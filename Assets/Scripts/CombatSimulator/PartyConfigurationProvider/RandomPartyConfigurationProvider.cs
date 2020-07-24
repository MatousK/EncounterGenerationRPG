using Assets.Scripts.Combat;

namespace Assets.Scripts.CombatSimulator.PartyConfigurationProvider
{
    /// <summary>
    /// The strategy which distributes power ups randomly to heroes.
    /// </summary>
    public class RandomPartyConfigurationProvider : PartyConfigurationProvider
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
            var healthPowerupDistribution = SplitPowerupsIntoGroups(powerups);
            var danagePowerupDistribution = SplitPowerupsIntoGroups(powerups);
            return new PartyConfiguration
            {
                ClericStats = GetStats(HeroProfession.Knight, healthPowerups: healthPowerupDistribution.ClericPowerups, attackPowerups: danagePowerupDistribution.ClericPowerups),
                KnightStats = GetStats(HeroProfession.Cleric, healthPowerups: healthPowerupDistribution.KnightPowerups, attackPowerups: danagePowerupDistribution.KnightPowerups),
                RangerStats = GetStats(HeroProfession.Ranger, healthPowerups: healthPowerupDistribution.RangerPowerups, attackPowerups: danagePowerupDistribution.RangerPowerups),
            };
        }
        /// <summary>
        /// Calculates randomly how many power ups should each character receive.
        /// </summary>
        /// <param name="powerupCount">Total number of power ups to distribute,</param>
        /// <returns>Distribution of power ups between individual characters.</returns>
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
        /// <summary>
        /// Defines how many power ups should each character receive.
        /// </summary>
        struct SplitResult
        {
            /// <summary>
            /// How many power ups should the cleric receive.
            /// </summary>
            public int ClericPowerups;
            /// <summary>
            /// How many power ups should the ranger receive.
            /// </summary>
            public int RangerPowerups;
            /// <summary>
            /// How many power ups should the knight receive.
            /// </summary>
            public int KnightPowerups;
        }
        /// <summary>
        /// Returns the string representation of this class.
        /// </summary>
        /// <returns>The string representation of this class.</returns>
        public override string ToString()
        {
            return "Random provider tier " + TierIndex.ToString();
        }
    }
}
