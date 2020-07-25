using System;
using Assets.Scripts.EncounterGenerator.Configuration;

namespace Assets.Scripts.EncounterGenerator.Model
{
    /// <summary>
    /// Represents a group of monsters of the same type that should be spawned in an encounter.
    /// </summary>
    [Serializable]
    public class MonsterGroup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MonsterGroup"/> class.
        /// </summary>
        public MonsterGroup()
        { }
        /// <summary>
        /// Initializes a new instance of the <see cref="MonsterGroup"/> class with specified parameters.
        /// </summary>
        /// <param name="monsterType">Type of monsters this group represents.</param>
        /// <param name="monsterCount">Number of monsters of this type in this group.</param>
        public MonsterGroup(MonsterType monsterType, int monsterCount)
        {
            MonsterType = monsterType;
            MonsterCount = monsterCount;
        }
        /// <summary>
        /// The type of monsters this monster group represents.
        /// </summary>
        public MonsterType MonsterType;
        /// <summary>
        /// How many monsters of this type should spawn.
        /// </summary>
        public int MonsterCount;
        /// <summary>
        /// Creates a clone of this class.
        /// </summary>
        /// <returns>The clone of this class.</returns>
        public MonsterGroup Clone()
        {
            return new MonsterGroup {
                MonsterType = MonsterType,
                MonsterCount = MonsterCount
            };
        }
        /// <summary>
        /// Check the equality between this and some target object.
        /// </summary>
        /// <param name="obj">Object to compare to.</param>
        /// <returns>True if the objects are equal, otherwise false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is MonsterGroup)
            {
                return (MonsterGroup)obj == this;
            }
            return false;
        }
        /// <summary>
        /// Retrieve the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override int GetHashCode()
        {
            return MonsterCount.GetHashCode() + MonsterType.GetHashCode() * 100;
        }
        /// <summary>
        /// Check the equality between two monster groups.
        /// </summary>
        /// <param name="first">First element of the comparison.</param>
        /// <param name="second">Second element of the comparison.</param>
        /// <returns>True if the objects are equal, otherwise false.</returns>
        public static bool operator ==(MonsterGroup first, MonsterGroup second)        {
            if (first is null || second is null)
            {
                return first is null == second is null;
            }
            return first.MonsterCount == second.MonsterCount && first.MonsterType == second.MonsterType;
        }
        /// <summary>
        /// Check the inequality between two monster groups.
        /// </summary>
        /// <param name="first">First element of the comparison.</param>
        /// <param name="second">Second element of the comparison.</param>
        /// <returns>True if the objects are not equal, otherwise false.</returns>
        public static bool operator !=(MonsterGroup first, MonsterGroup second)
        {
            return !(first == second);
        }
        /// <summary>
        /// Retrieve the adjusted monster count of this monster group, i.e. sum of weights of all monsters in this group.
        /// </summary>
        /// <param name="configuration">General configuration for the encounter generator.</param>
        /// <returns>The adjusted monster count of this group.</returns>
        public float GetAdjustedMonsterCount(EncounterGeneratorConfiguration configuration)
        {
            return configuration.MonsterRankWeights[MonsterType] * MonsterCount;
        }
    }
}