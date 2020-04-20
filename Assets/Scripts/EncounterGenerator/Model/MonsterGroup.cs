using System;
using Assets.Scripts.EncounterGenerator.Configuration;

namespace Assets.Scripts.EncounterGenerator.Model
{
    /// <summary>
    /// Represents a group of monsters of the same type that might be spawned in an encounter.
    /// </summary>
    [Serializable]
    public class MonsterGroup
    {
        public MonsterGroup()
        { }

        public MonsterGroup(MonsterType monsterType, int monsterCount)
        {
            MonsterType = monsterType;
            MonsterCount = monsterCount;
        }
        public MonsterType MonsterType;
        public int MonsterCount;

        public MonsterGroup Clone()
        {
            return new MonsterGroup {
                MonsterType = MonsterType,
                MonsterCount = MonsterCount
            };
        }

        public override bool Equals(object obj)
        {
            if (obj is MonsterGroup)
            {
                return (MonsterGroup)obj == this;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return MonsterCount.GetHashCode() + MonsterType.GetHashCode() * 100;
        }

        public static bool operator ==(MonsterGroup first, MonsterGroup second)        {
            if (first is null || second is null)
            {
                return first is null == second is null;
            }
            return first.MonsterCount == second.MonsterCount && first.MonsterType == second.MonsterType;
        }

        public static bool operator !=(MonsterGroup first, MonsterGroup second)
        {
            return !(first == second);
        }

        public float GetAdjustedMonsterCount(EncounterGeneratorConfiguration configuration)
        {
            return configuration.MonsterRankWeights[MonsterType] * MonsterCount;
        }
    }
}