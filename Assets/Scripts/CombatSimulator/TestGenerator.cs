using EncounterGenerator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class TestGenerator
{
    int testIndex = 0;
    public void ReadyNextTest()
    {
        testIndex++;
    }

    public PartyConfiguration GetCurrentPartyConfiguration()
    {
        return new PartyConfiguration
        {
            KnightStats = new PartyMemberConfiguration { MaxHP = 250, AttackModifier = 15 },
            RangerStats = new PartyMemberConfiguration { MaxHP = 50, AttackModifier = 30 },
            ClericStats = new PartyMemberConfiguration { MaxHP = 125, AttackModifier = 10 },
        };
    }

    public EncounterDefinition GetCurrentTestEncounter()
    {
        return new EncounterDefinition
        {
            AllEncounterGroups = new List<MonsterGroup>
            {
                new MonsterGroup
                {
                    MonsterType = new MonsterType(MonsterRank.Elite, MonsterRole.Leader),
                    MonsterCount = 1
                },
                new MonsterGroup
                {
                    MonsterType = new MonsterType(MonsterRank.Regular, MonsterRole.Brute),
                    MonsterCount = 1
                },
                new MonsterGroup
                {
                    MonsterType = new MonsterType(MonsterRank.Minion, MonsterRole.Minion),
                    MonsterCount = testIndex * 4
                }
            }
        };
    }

}

public struct PartyConfiguration
{
    public PartyMemberConfiguration KnightStats;
    public PartyMemberConfiguration RangerStats;
    public PartyMemberConfiguration ClericStats;
}

public struct PartyMemberConfiguration
{
    public int MaxHP;
    public float AttackModifier;
}