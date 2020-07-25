using System;
using Assets.Scripts.Combat;

namespace Assets.Scripts.EncounterGenerator.Model
{
    /// <summary>
    /// Monster type are data about a monster relevant to the encounter generator.
    /// In Thesis Quest, this is the monster's rank and role.
    /// </summary>
    [Serializable]
    public struct MonsterType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MonsterType"/> class.
        /// </summary>
        /// <param name="rank">Rank of the monster type.</param>
        /// <param name="role">Role of the monster type.</param>
        public MonsterType(MonsterRank rank, MonsterRole role)
        {
            Rank = rank;
            Role = role;
        }
        /// <summary>
        /// Rank of the monster this monster type represents, i.e. how strong it is.
        /// </summary>
        public MonsterRank Rank;
        /// <summary>
        /// Role of the monster this monster type represents, i.e. what is its role in an encounter.
        /// </summary>
        public MonsterRole Role;
        /// <summary>
        /// Check the equality between this and some target object.
        /// </summary>
        /// <param name="obj">Object to compare to.</param>
        /// <returns>True if the objects are equal, otherwise false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is MonsterType)
            {
                return (MonsterType)obj == this;
            }
            return false;
        }
        /// <summary>
        /// Retrieve the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override int GetHashCode()
        {
            return Rank.GetHashCode() * 10000 + Role.GetHashCode();
        }
        /// <summary>
        /// Check the equality between two monster types.
        /// </summary>
        /// <param name="first">First element of the comparison.</param>
        /// <param name="second">Second element of the comparison.</param>
        /// <returns>True if the objects are equal, otherwise false.</returns>
        public static bool operator ==(MonsterType first, MonsterType second)
        {
            return first.Role == second.Role && first.Rank == second.Rank;
        }
        /// <summary>
        /// Check the inequality between two monster types.
        /// </summary>
        /// <param name="first">First element of the comparison.</param>
        /// <param name="second">Second element of the comparison.</param>
        /// <returns>True if the objects are not equal, otherwise false.</returns>
        public static bool operator !=(MonsterType first, MonsterType second)
        {
            return !(first == second);
        }
    }
}