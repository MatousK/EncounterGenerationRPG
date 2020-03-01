using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncounterGenerator.Configuration
{
    public class EncounterGeneratorConfiguration
    {
        /// <summary>
        /// How to individual roles correspond to either defense or offense of the group.
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
        /// How many regular monsters is the specified monster rank worth.
        /// </summary>
        public Dictionary<MonsterRank, float> MonsterRankWeights = new Dictionary<MonsterRank, float>
        {
            { MonsterRank.Minion, 0.2f },
            { MonsterRank.Regular, 1f },
            { MonsterRank.Elite, 2f },
            { MonsterRank.Boss, 4f },
        };

        /// <summary>
        /// How much should the algorithm try to change its data based on actual battles. 0 means no learning at all, 1 means that it should completely change the matrix to fit the data.
        /// </summary>
        public float LearningSpeed;
    }
}
