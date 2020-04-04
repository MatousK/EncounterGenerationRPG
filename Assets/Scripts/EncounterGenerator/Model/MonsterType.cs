using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace EncounterGenerator.Model
{
    // In our case, monster's type is defined by its rank and role, e.g. Elite Sniper.
    [Serializable]
    public struct MonsterType
    {
        public MonsterType(MonsterRank rank, MonsterRole role)
        {
            Rank = rank;
            Role = role;
        }
        public MonsterRank Rank;
        public MonsterRole Role;

        public override bool Equals(object obj)
        {
            if (obj is MonsterType)
            {
                return (MonsterType)obj == this;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Rank.GetHashCode() * 10000 + Role.GetHashCode();
        }


        public static bool operator ==(MonsterType first, MonsterType second)
        {
            return first.Role == second.Role && first.Rank == second.Rank;
        }

        public static bool operator !=(MonsterType first, MonsterType second)
        {
            return !(first == second);
        }
    }
}