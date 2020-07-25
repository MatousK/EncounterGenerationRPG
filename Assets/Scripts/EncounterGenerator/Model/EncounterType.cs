namespace Assets.Scripts.EncounterGenerator.Model
{
    /// <summary>
    /// This class represents a high level type of encounter to track variance.
    /// It would be quite complex to track all different encounters to ensure variance.
    /// Instead, we will try to track some general criteria described in this class.
    /// </summary>
    public class EncounterType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EncounterType"/> class
        /// </summary>
        /// <param name="hasLeader">If true, leader should be spawned in encounters of this type.</param>
        /// <param name="attackDefenseRatio">How is should the encounter be balanced when talking about offense and defense. The higher the value, the more attack oriented the characters.</param>
        /// <param name="spawnBossIfPossible">If true, a boss should be spawned in encounters of this type.</param>
        public EncounterType(bool hasLeader, float attackDefenseRatio, bool spawnBossIfPossible)
        {
            HasLeader = hasLeader;
            AttackDefenseRatio = attackDefenseRatio;
            SpawnBossIfPossible = spawnBossIfPossible;
        }
        /// <summary>
        /// If true, leader should be spawned in encounters of this type.
        /// </summary>
        public bool HasLeader;
        /// <summary>
        /// How is should the encounter be balanced when talking about offense and defense. The higher the value, the more attack oriented the characters.
        /// </summary>
        public float AttackDefenseRatio;
        /// <summary>
        /// If true, a boss should be spawned in encounters of this type.
        /// </summary>
        public bool SpawnBossIfPossible;
    }
}