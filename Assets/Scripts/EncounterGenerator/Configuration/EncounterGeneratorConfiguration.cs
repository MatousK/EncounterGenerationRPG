using System.Collections.Generic;
using Assets.Scripts.Combat;
using Assets.Scripts.EncounterGenerator.Model;

namespace Assets.Scripts.EncounterGenerator.Configuration
{
    /// <summary>
    /// The general configuration for the algorithm.
    /// </summary>
    public class EncounterGeneratorConfiguration
    {
        /// <summary>
        /// Configuration with the bug which generated invalid monsters. We keep it so we can emulate the bug when analyzing old data.
        /// </summary>
        public static EncounterGeneratorConfiguration ConfigurationV1 = new EncounterGeneratorConfiguration
        {
            LearningSpeedDecreaseDifficulty = 0.5f,
            LearningSpeedIncreaseDifficulty = 0.5f,
            EmulateV1Bug = true
        };
        /// <summary>
        /// Configuration which fixed the bugs from V1.
        /// </summary>
        public static EncounterGeneratorConfiguration ConfigurationV2 = new EncounterGeneratorConfiguration();
        /// <summary>
        /// The configuration that should be used by the game.
        /// </summary>
        public static EncounterGeneratorConfiguration CurrentConfig
        {
            get
            {
                return ConfigurationV2;
            }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="EncounterGeneratorConfiguration"/>.
        /// </summary>
        private EncounterGeneratorConfiguration() { }
        /// <summary>
        /// For each role of monster, we calculate how attack or defense oriented the monster is.
        /// The higher the value, the more attack the monster is oriented, i.e., the higher the attack compared to health. 
        /// </summary>
        public Dictionary<MonsterRole, float> MonsterRoleAttackDefenseRatios = new Dictionary<MonsterRole, float>
        {
            { MonsterRole.Brute, 0.5f },
            { MonsterRole.Leader,  1f },
            { MonsterRole.Lurker,  3f },
            { MonsterRole.Minion,  1f },
            { MonsterRole.Sniper,  2f },
        };

        /// <summary>
        /// For each monster we calculate a number specifying how dangerous is.
        /// Should be in the same order of magnitude as party strength.
        /// </summary>
        public Dictionary<MonsterType, float> MonsterRankWeights = new Dictionary<MonsterType, float>
        {
            // The weights were determined by some basic statistical analysis. 
            // We tried to build a linear model that would predict the strengths of individual enemies. While the exact numbers were not usable for us,
            // they did seem to match our intuitive understanding of how powerful should the individual monsters be in relation to each other.
            { new MonsterType(MonsterRank.Minion, MonsterRole.Minion), 110f },
            { new MonsterType(MonsterRank.Regular, MonsterRole.Brute), 320f },
            { new MonsterType(MonsterRank.Elite, MonsterRole.Brute), 710f },
            { new MonsterType(MonsterRank.Boss, MonsterRole.Brute), 1860f },
            { new MonsterType(MonsterRank.Regular, MonsterRole.Leader), 0f }, // Regular leaders do not exist.
            { new MonsterType(MonsterRank.Elite, MonsterRole.Leader), 790f },
            { new MonsterType(MonsterRank.Boss, MonsterRole.Leader), 1470f },
            { new MonsterType(MonsterRank.Regular, MonsterRole.Lurker), 290f },
            { new MonsterType(MonsterRank.Elite, MonsterRole.Lurker), 930f },
            { new MonsterType(MonsterRank.Boss, MonsterRole.Lurker), 2060f },
            { new MonsterType(MonsterRank.Regular, MonsterRole.Sniper), 0470f },
            { new MonsterType(MonsterRank.Elite, MonsterRole.Sniper), 960f },
            { new MonsterType(MonsterRank.Boss, MonsterRole.Sniper), 2050f },
        };
        // Weights from the smart AI matrix, not used. Also, these were not scaled to the party strength.
        //public Dictionary<MonsterType, float> MonsterRankWeights = new Dictionary<MonsterType, float>
        //{
        //    // The weights were determined by some basic statistical analysis,
        //    { new MonsterType(MonsterRank.Minion, MonsterRole.Minion), 19f },
        //    { new MonsterType(MonsterRank.Regular, MonsterRole.Brute), 28f },
        //    { new MonsterType(MonsterRank.Elite, MonsterRole.Brute), 70f },
        //    { new MonsterType(MonsterRank.Boss, MonsterRole.Brute), 136f },
        //    { new MonsterType(MonsterRank.Regular, MonsterRole.Leader), 0f }, // Regular leaders do not exist.
        //    { new MonsterType(MonsterRank.Elite, MonsterRole.Leader), 48f },
        //    { new MonsterType(MonsterRank.Boss, MonsterRole.Leader), 113f },
        //    { new MonsterType(MonsterRank.Regular, MonsterRole.Lurker), 50f },
        //    { new MonsterType(MonsterRank.Elite, MonsterRole.Lurker), 84f },
        //    { new MonsterType(MonsterRank.Boss, MonsterRole.Lurker), 146f },
        //    { new MonsterType(MonsterRank.Regular, MonsterRole.Sniper), 64f },
        //    { new MonsterType(MonsterRank.Elite, MonsterRole.Sniper), 94f },
        //    { new MonsterType(MonsterRank.Boss, MonsterRole.Sniper), 147f },
        //};

        /// <summary>
        /// How much should the algorithm try to change its data based on actual battles when the battle is easier than it should have been.
        /// 0 means no learning at all, 1 means that it should change the matrix as much as possible to fit the data.
        /// </summary>
        public float LearningSpeedIncreaseDifficulty = 0.75f;
        /// <summary>
        /// How much should the algorithm try to change its data based on actual battles when the battle is harder than it should have been.
        /// 0 means no learning at all, 1 means that it should change the matrix as much as possible to fit the data.
        /// </summary>
        public float LearningSpeedDecreaseDifficulty = 0.75f;
        /// <summary>
        /// While learning, every field in the matrix should be rated as at least this similar.
        /// Why? Because every fight tells us something about the player's ability, so it should affect the entire matrix by at least some amount.
        /// After some testing, we decided not to use the parameter and keep it at zero, thereby disabling the behavior.
        /// </summary>
        public float LearningMinimumSimilarity = 0f;
        /// <summary>
        /// This is for analyzing old data with bugs.
        /// If true, we will emulate the wrong behavior of matrix adjustments that led to invalid data.
        /// </summary>
        public bool EmulateV1Bug;
    }
}
