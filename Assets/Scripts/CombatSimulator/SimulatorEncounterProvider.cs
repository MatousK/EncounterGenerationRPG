using System.Collections.Generic;
using Assets.Scripts.Combat;
using Assets.Scripts.EncounterGenerator.Model;
using Assets.Scripts.Extension;

namespace Assets.Scripts.CombatSimulator
{
    /// <summary>
    /// This class can create an encounter for some specific amount of monsters to be spawned.
    /// </summary>
    class SimulatorEncounterProvider
    {
        /// <summary>
        /// How many individual monsters can be spawned at once. This is the non adjusted count, so a boss is just one monster.
        /// However, 4 minions ARE just one monster. Not really sure why I implemented it this way, should be refactored at some point :)
        /// </summary>
        const int MaxMonsterCount = 10;
        /// <summary>
        /// Gets an encounter with the specified amount of monsters. Note that the count is adjusted, see the description of the parameter.
        /// </summary>
        /// <param name="adjustedMonsterCount">The amount of monsters that should spawn. Adjusted means that a boss is worth 4 monsters, an elite is worth 2 monsters, normal creatures are worth 1 monster and 4 minions are worth one normal monster.</param>
        /// <returns>The generated encounter definition.</returns>
        public EncounterDefinition GetEncounter(int adjustedMonsterCount)
        {
            int remainingAdjustedMonsterCount = adjustedMonsterCount;
            int remainingMaxMonsterCount = MaxMonsterCount;

            Dictionary<MonsterType, int> monstersToSpawn = new Dictionary<MonsterType, int>();

            var allMonsterTypes = GetMonsterTypeList();
            var spawnLeader = UnityEngine.Random.Range(0f, 1f) < 0.5f;
            if (spawnLeader)
            {
                AddLeader(monstersToSpawn, ref remainingAdjustedMonsterCount);
                remainingMaxMonsterCount--;
            }
            // To make sure we do not get stuck in a loop in some edge cases, count iterations and break if generating for too long.
            int maxIterationCount = 100000;
            int currentIteration = 0;
            /// Try adding monsters while we can.
            while (remainingAdjustedMonsterCount > 0 && currentIteration++ < maxIterationCount)
            {
                // Find the monster that should spawn and spawn it.
                var monsterCandidate = allMonsterTypes.GetRandomElementOrDefault();
                var monsterWeight = GetMonsterWeight(monsterCandidate);
                // First condition makes sure that we do not make the encounter more difficult than we thought.
                // Second ensures we do not spawn too many monsters. If the condition is true, that means that if we were only adding monsters of this weight or easier,
                // we would still overshoot. Therefore, we must spawn at least one monster of some higher difficulty, so we do that right now.
                if (monsterWeight > remainingAdjustedMonsterCount || remainingAdjustedMonsterCount > monsterWeight * remainingMaxMonsterCount)
                {
                    continue;
                }
                var oldValue = monstersToSpawn.ContainsKey(monsterCandidate) ? monstersToSpawn[monsterCandidate] : 0;
                monstersToSpawn[monsterCandidate] = oldValue + 1;
                remainingAdjustedMonsterCount -= monsterWeight;
                remainingMaxMonsterCount--;
            }
            // Got list of monsters, make an encounter out of it.
            EncounterDefinition toReturn = new EncounterDefinition { AllEncounterGroups = new List<MonsterGroup>() };
            foreach (var entry in monstersToSpawn)
            {
                // Each minion
                var monsterCount = entry.Key.Rank == MonsterRank.Minion ? 4 * entry.Value : entry.Value;
                toReturn.AllEncounterGroups.Add(new MonsterGroup { MonsterType = entry.Key, MonsterCount = monsterCount });
            }
            return toReturn;
        }
        /// <summary>
        /// Retrieve the our internal weight rank of the monster.
        /// </summary>
        /// <param name="monster">Monster whose weight is requested.</param>
        /// <returns>The weight of the monster.</returns>
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
        /// <summary>
        /// Adds the leader to an encounter.
        /// Will not spawn boss if less than 5 monsters are requested, as the leader always needs at least one ally to be effective.
        /// </summary>
        /// <param name="monstersToSpawn">Monsters that will be spawned. Output parameter, will be modified.</param>
        /// <param name="remainingMonsterCount">How many monsters can still be spawned. Output parameter, will be modified.</param>
        void AddLeader(Dictionary<MonsterType, int> monstersToSpawn, ref int remainingMonsterCount)
        {
            bool canAddBoss = remainingMonsterCount > 4;
            bool addingBoss = canAddBoss && UnityEngine.Random.Range(0f, 1f) < 0.5;
            var leaderMonsterType = new MonsterType(addingBoss ? MonsterRank.Boss : MonsterRank.Elite, MonsterRole.Leader);
            monstersToSpawn[leaderMonsterType] = 1;
            remainingMonsterCount -= addingBoss ? 4 : 2;
        }
        /// <summary>
        /// Retrieve the list of all monsters that can be spawned.
        /// Leaders are not in this list, as they are handled separately.
        /// </summary>
        /// <returns>The list of monster types that can be spawned.</returns>
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