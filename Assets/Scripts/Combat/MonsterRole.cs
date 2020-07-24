namespace Assets.Scripts.Combat
{
    /// <summary>
    /// Role of a monster in combat.
    /// </summary>
    public enum MonsterRole
    {
        /// <summary>
        /// Ranged attacker.
        /// </summary>
        Sniper,
        /// <summary>
        /// Frontline melee fighter.
        /// </summary>
        Brute,
        /// <summary>
        /// Thief with high damage killing high priority targets.
        /// </summary>
        Lurker,
        /// <summary>
        /// Weak enemy with low attack and HP.
        /// </summary>
        Minion,
        /// <summary>
        /// Commands other creatures to attack its targets. Can also heal allies.
        /// </summary>
        Leader
    }
}
