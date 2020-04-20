using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Combat;
using Assets.Scripts.EncounterGenerator.Configuration;
using Assets.Scripts.EncounterGenerator.Model;
using Assets.Scripts.Extension;

namespace Assets.Scripts.EncounterGenerator.Algorithm
{
    // This class can create an encounter definition based on an encounter type. It does not keep history at this moment.
    class EncounterDefinitionManager
    {
        public EncounterDefinitionManager(EncounterGeneratorConfiguration configuration, List<MonsterType> availableMonsterTypes, EncounterType encounterType)
        {
            this.configuration = configuration;
            this.availableMonsterTypes = availableMonsterTypes;
            this.encounterType = encounterType;
        }

        readonly EncounterGeneratorConfiguration configuration;
        readonly List<MonsterType> availableMonsterTypes;
        readonly EncounterType encounterType;

        /// <summary>
        /// Creates an encounter with similar overall difficulty as the encounter provided.
        /// The difficulty is estimated only by the number of monsters generated.
        /// </summary>
        /// <param name="exampleEncounter">The encounter close to the requested difficulty.</param>
        /// <returns></returns>
        public EncounterDefinition GenerateEncounter(EncounterDefinition exampleEncounter)
        {
            var adjustedMonsterCountToGenerate = exampleEncounter.GetAdjustedMonsterCount(configuration);
            var toReturn = new EncounterDefinition { AllEncounterGroups = new List<MonsterGroup>() };
            float currentAdjustedMonsterWeight = 0;
            // We are only adding new and new monsters, so we cann
            while (currentAdjustedMonsterWeight < adjustedMonsterCountToGenerate)
            {
                AddValidMonsterToEncounter(toReturn, adjustedMonsterCountToGenerate - currentAdjustedMonsterWeight);
                currentAdjustedMonsterWeight = toReturn.GetAdjustedMonsterCount(configuration);
            }
            return toReturn;
        }
        /// <summary>
        /// Tweaks the difficulty of the specified encounter  
        /// </summary>
        /// <param name="adjustedEncounter"> The encounter to adjust.</param>
        /// <param name="shouldIncreaseDifficulty"> If true, the difficulty of the encounter should be increased, otherwise false.</param>
        public void AdjustEncounter(EncounterDefinition adjustedEncounter, bool shouldIncreaseDifficulty)
        {
            if (shouldIncreaseDifficulty)
            {
                // We do not care about strength of the monster. If we overshoot, we can go back again.
                AddValidMonsterToEncounter(adjustedEncounter, float.MaxValue);
            }
            else
            {
                MakeEncounterEasier(adjustedEncounter);
            }
        }

        private void MakeEncounterEasier(EncounterDefinition encounter)
        {
            // There are two main ways to make an encounter easier - decrease of a rank or remove a monster.
            var eliteRankDownCandidates = encounter.AllEncounterGroups.Where(group => group.MonsterType.Rank == MonsterRank.Elite && CanDowngradeMonster(group.MonsterType)).ToArray();
            // Bosses and leaders are important and should not be removed unless absolutely necessary.
            var removeMonsterCandidates = encounter.AllEncounterGroups.Where(group => group.MonsterType.Rank != MonsterRank.Boss && group.MonsterType.Role != MonsterRole.Leader).ToArray();

            // It is apparent that if a monster can be downgraded, the same monster can also be removed.
            // If we can rank down a monster, we make it fifty/fifty whether we downgrade a monster or remove it.
            if (eliteRankDownCandidates.Any() && UnityEngine.Random.value < 0.5)
            {
                DowngradeMonster(encounter, eliteRankDownCandidates.GetRandomElementOrDefault().MonsterType);
            }
            else if (removeMonsterCandidates.Any())
            {
                RemoveSpecificMonsterFromEncounter(encounter, removeMonsterCandidates.GetRandomElementOrDefault().MonsterType);
            }
            else
            {
                // The encounter is apparently only a leader and/or a boss. Welp, too bad. If there is a boss, downgrade it or remove it. If there is just a leader, well... Nothing to be done.
                // Single elite leader should be beatable by absolutely anyone.
                // So try to find a boss and downgrade/remove it.
                var boss = encounter.AllEncounterGroups.First(group => group.MonsterType.Rank == MonsterRank.Boss);
                if (boss == null)
                {
                    return; // Probably only a boss in this encounter. Nothing to be done.
                }
                if (CanDowngradeMonster(boss.MonsterType))
                {
                    DowngradeMonster(encounter, boss.MonsterType);
                }
                else
                {
                    RemoveSpecificMonsterFromEncounter(encounter, boss.MonsterType);
                }
            }
        }

        private void DowngradeMonster(EncounterDefinition encounter, MonsterType toDowngrade)
        {
            RemoveSpecificMonsterFromEncounter(encounter, toDowngrade);
            var downgradedType = toDowngrade;
            downgradedType.Rank--;
            AddSpecificMonsterToEncounter(encounter, downgradedType);
        }

        private void AddValidMonsterToEncounter(EncounterDefinition encounter, float maxMonsterWeight)
        {
            var hasBoss = encounter.AllEncounterGroups.Any(group => group.MonsterType.Rank == MonsterRank.Boss);
            var hasLeader = encounter.AllEncounterGroups.Any(group => group.MonsterType.Role == MonsterRole.Leader);
            var currentOffenseDefenseRatio = encounter.GetAttackDefenseRatio(configuration);

            var candidates = new List<MonsterType>(availableMonsterTypes);
            candidates = FilterCandidatesResettingIfEmpty(candidates, candidate => configuration.MonsterRankWeights[candidate.Rank] <= maxMonsterWeight);

            bool shouldSpawnLeader = !hasLeader && encounterType.HasLeader;
            candidates = FilterCandidatesResettingIfEmpty(candidates, candidate => (candidate.Role == MonsterRole.Leader) == shouldSpawnLeader);

            bool shouldSpawnBoss = !hasBoss && encounterType.SpawnBossIfPossible;
            candidates = FilterCandidatesResettingIfEmpty(candidates, candidate => (candidate.Rank == MonsterRank.Boss) == shouldSpawnBoss);

            var offenseDefenseRatioDifference = encounterType.AttackDefenseRatio - currentOffenseDefenseRatio;
            if (offenseDefenseRatioDifference < 0)
            {
                // Too attack oriented, spawn something more defense oriented.
                FilterCandidatesResettingIfEmpty(candidates, candidate => configuration.MonsterRoleAttackDefenseRatios[candidate.Role] <= currentOffenseDefenseRatio);
            }
            else
            {
                // Too defense oriented, spawn something attack oriented.
                FilterCandidatesResettingIfEmpty(candidates, candidate => configuration.MonsterRoleAttackDefenseRatios[candidate.Role] >= currentOffenseDefenseRatio);
            }

            // Okay, so now we hopefully have a list of valid candidates.  Pick one.
            // TODO: Bigger probabilities for those close to to what we want.
            var selectedMonster = candidates.GetRandomElementOrDefault();
            AddSpecificMonsterToEncounter(encounter, selectedMonster);   
        }

        private void RemoveSpecificMonsterFromEncounter(EncounterDefinition encounter, MonsterType monsterToRemove)
        {
            for (int i = 0; i < encounter.AllEncounterGroups.Count; ++i)
            {
                if (encounter.AllEncounterGroups[i].MonsterType == monsterToRemove)
                {
                    var selectedMonsterWeight = configuration.MonsterRankWeights[monsterToRemove.Rank];
                    encounter.AllEncounterGroups[i].MonsterCount -= selectedMonsterWeight < 1 ? (int)(1f / selectedMonsterWeight) : 1;
                    if (encounter.AllEncounterGroups[i].MonsterCount <= 0)
                    {
                        encounter.AllEncounterGroups.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        private void AddSpecificMonsterToEncounter(EncounterDefinition encounter, MonsterType monsterToAdd)
        {
            var relevantGroup = encounter.AllEncounterGroups.FirstOrDefault(group => group.MonsterType == monsterToAdd);
            if (relevantGroup == null)
            {
                relevantGroup = new MonsterGroup { MonsterType = monsterToAdd };
                encounter.AllEncounterGroups.Add(relevantGroup);
            }
            var selectedMonsterWeight = configuration.MonsterRankWeights[monsterToAdd.Rank];
            // If adding minions (or any other monster with below one weight), add it enough times that it is one standard monster
            relevantGroup.MonsterCount += selectedMonsterWeight < 1 ? (int)(1 / selectedMonsterWeight) : 1;
        }

        private List<MonsterType> FilterCandidatesResettingIfEmpty(List<MonsterType> candidates, Func<MonsterType, bool> predicate)
        {
            var candidatesBackup = new List<MonsterType>(candidates);
            // Try to filter based on the predicate.
            candidates = candidates.Where(predicate).ToList();
            // If no matches were found, reset to previous state.
            return candidates.Any() ? candidates : candidatesBackup;
        }
        /// <summary>
        /// Checks if the specified monster can be downgraded. Works only for elite and boss monsters.
        /// </summary>
        /// <param name="toDowngrade">Monster type to downgrade</param>
        /// <returns></returns>
        private bool CanDowngradeMonster(MonsterType toDowngrade)
        {
            var newRank = toDowngrade.Rank - 1;
            return availableMonsterTypes.Any(monsterType => monsterType.Rank == MonsterRank.Regular && monsterType.Role == toDowngrade.Role);
        }
    }
}