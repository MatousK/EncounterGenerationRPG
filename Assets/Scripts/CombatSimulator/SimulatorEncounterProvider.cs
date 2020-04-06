using System.Collections.Generic;
using Assets.Scripts.Combat;
using Assets.Scripts.EncounterGenerator.Model;
using Assets.Scripts.Extension;

namespace Assets.Scripts.CombatSimulator
{
    class SimulatorEncounterProvider
    {
        const int MaxMonsterCount = 10;
        public EncounterDefinition GetEncounter(int adjustedMonsterCount)
        {
            int remainingMonsterCount = adjustedMonsterCount;

            Dictionary<MonsterType, int> monstersToSpawn = new Dictionary<MonsterType, int>();

            var allMonsterTypes = GetMonsterTypeList();
            var spawnLeader = UnityEngine.Random.Range(0f, 1f) < 0.5f;
            if (spawnLeader)
            {
                AddLeader(monstersToSpawn, ref remainingMonsterCount);
            }
            // To make sure we do not get stuck in a loop in some edge cases, count iterations and break if generating for too long.
            int maxIterationCount = 100000;
            int currentIteration = 0;
            while (remainingMonsterCount > 0 && currentIteration++ < maxIterationCount)
            {
                var monsterCandidate = allMonsterTypes.GetRandomElementOrDefault();
                var monsterWeight = GetMonsterWeight(monsterCandidate);
                // First condition makes sure that we do not make the encounter more difficult than we thought,
                if (monsterWeight > remainingMonsterCount || remainingMonsterCount > monsterWeight * MaxMonsterCount)
                {
                    continue;
                }
                var oldValue = monstersToSpawn.ContainsKey(monsterCandidate) ? monstersToSpawn[monsterCandidate] : 0;
                monstersToSpawn[monsterCandidate] = oldValue + 1;
                remainingMonsterCount -= monsterWeight;
            }
            // Got list of monsters, make an encounter out of it.
            EncounterDefinition toReturn = new EncounterDefinition { AllEncounterGroups = new List<MonsterGroup>() };
            foreach (var entry in monstersToSpawn)
            {
                var monsterCount = entry.Key.Rank == MonsterRank.Minion ? 4 * entry.Value : entry.Value;
                toReturn.AllEncounterGroups.Add(new MonsterGroup { MonsterType = entry.Key, MonsterCount = monsterCount });
            }
            return toReturn;
        }

        int GetMonsterWeight(MonsterType monster)
        {
            switch (monster.Rank)
            {
                case MonsterRank.Boss:
                    return 4;
                case MonsterRank.Elite:
                    return 2;
                default:
                    return 1;
            }
        }

        void AddLeader(Dictionary<MonsterType, int> monstersToSpawn, ref int remainingMonsterCount)
        {
            bool canAddBoss = remainingMonsterCount > 4;
            bool addingBoss = canAddBoss && UnityEngine.Random.Range(0f, 1f) < 0.5;
            var leaderMonsterType = new MonsterType(addingBoss ? MonsterRank.Boss : MonsterRank.Elite, MonsterRole.Leader);
            monstersToSpawn[leaderMonsterType] = 1;
            remainingMonsterCount -= addingBoss ? 4 : 2;
        }

        List<MonsterType> GetMonsterTypeList()
        {
            //Initialize with monster types that do not exist for every rank. Do not add leaders, we handle them separately.
            var toReturn = new List<MonsterType> { new MonsterType(MonsterRank.Minion, MonsterRole.Minion) };
            var monsterRanks = new List<MonsterRank> { MonsterRank.Boss, MonsterRank.Elite, MonsterRank.Regular };
            var monsterRoles = new List<MonsterRole> { MonsterRole.Brute, MonsterRole.Lurker, MonsterRole.Sniper };
            foreach (var rank in monsterRanks)
            {
                foreach (var role in monsterRoles)
                {
                    toReturn.Add(new MonsterType(rank, role));
                }
            }
            return toReturn;
        }
    }
}