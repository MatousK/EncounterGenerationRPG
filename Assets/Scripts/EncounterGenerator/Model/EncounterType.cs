using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncounterGenerator.Model
{
    /// <summary>
    /// This class represents a high level type of encounter to track variance.
    /// It would be quite complex to track all different encounter to ensure variance.
    /// Instead, we will try to track some general criteria dscribed in this class.
    /// </summary>
    public class EncounterType
    {
        public EncounterType(bool hasLeader, float attackDefenseRatio, bool spawnBossIfPossible)
        {
            HasLeader = hasLeader;
            AttackDefenseRatio = attackDefenseRatio;
            SpawnBossIfPossible = spawnBossIfPossible;
        }
        /// <summary>
        /// If true, leader should be spawned as a part of this encounter.
        /// </summary>
        public bool HasLeader;
        /// <summary>
        /// How is this group balanced when talking about offense and defense.
        /// </summary>
        public float AttackDefenseRatio;
        /// <summary>
        /// The algorithm will try to spawn a boss in this combat if at all possible. If false, bosses are way less likely to appear.
        /// </summary>
        public bool SpawnBossIfPossible;
    }
}