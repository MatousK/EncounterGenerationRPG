namespace Assets.Scripts.Combat
{
    /// <summary>
    /// A rank of a monster, signifying its strength.
    /// </summary>
    public enum MonsterRank
    {
        /// <summary>
        /// Normal enemy.
        /// </summary>
        Regular,
        /// <summary>
        /// Twice as strong as normal enemy.
        /// </summary>
        Elite, 
        /// <summary>
        /// Twice as strong as elite enemy.
        /// </summary>
        Boss,
        /// <summary>
        /// Very weak enemy who should die in one hit.
        /// </summary>
        Minion
    }
}