using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Combat;
using Assets.Scripts.EncounterGenerator.Configuration;
using Assets.Scripts.EncounterGenerator.Model;
using Assets.Scripts.Extension;

namespace Assets.Scripts.EncounterGenerator.Algorithm
{
    /// <summary>
    /// This class can create an initial encounter for the hill climbing algorithm and then adjust it as required.
    /// </summary>
    class EncounterDefinitionManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EncounterDefinitionManager"/> class.
        /// </summary>
        /// <param name="configuration">The current configuration for the encounter generator.</param>
        /// <param name="availableMonsterTypes">Types of monsters that are on the allowed monsters list and can be generated.</param>
        /// <param name="encounterType">Type of encounter we should be trying to create.</param>
        public EncounterDefinitionManager(EncounterGeneratorConfiguration configuration, List<MonsterType> availableMonsterTypes, EncounterType encounterType)
        {
            this.configuration = configuration;
            this.availableMonsterTypes = availableMonsterTypes;
            this.encounterType = encounterType;
        }
        /// <summary>
        /// The current configuration for the encounter generator.
        /// </summary>
        readonly EncounterGeneratorConfiguration configuration;
        /// <summary>
        /// Types of monsters that are on the allowed monsters list and can be generated.
        /// </summary>
        readonly List<MonsterType> availableMonsterTypes;
        /// <summary>
        /// Type of encounter we should be trying to create.
        /// </summary>
        readonly EncounterType encounterType;

        /// <summary>
        /// Creates an encounter with similar overall difficulty as the encounter provided.
        /// The difficulty is estimated only by the sum of weights of monsters generated.
        /// However, this encounter will be of the <see cref="encounterType"/> type.
        /// </summary>
        /// <param name="exampleEncounter">The encounter close to the requested difficulty.</param>
        /// <returns>The encounter of similar difficulty but with correct type.</returns>
        public EncounterDefinition GenerateEncounter(EncounterDefinition exampleEncounter)
        {
            var adjustedMonsterCountToGenerate = exampleEncounter.GetAdjustedMonsterCount(configuration);
            var toReturn = new EncounterDefinition { AllEncounterGroups = new List<MonsterGroup>() };
            float currentAdjustedMonsterWeight = 0;
            // We are only adding new and new monsters, we can remove them later if we overshoot. We only need to be close right now, not perfect.
            while (currentAdjustedMonsterWeight < adjustedMonsterCountToGenerate)
            {
                AddValidMonsterToEncounter(toReturn, adjustedMonsterCountToGenerate - currentAdjustedMonsterWeight);
                currentAdjustedMonsterWeight = toReturn.GetAdjustedMonsterCount(configuration);
                toReturn.UpdatePrecomputedMonsterCount(configuration);
            }
            return toReturn;
        }
        /// <summary>
        /// Tweaks the difficulty of the specified encounter by adding or removing monsters from it.
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
        /// <summary>
        /// This method decreases an encounter difficulty by either downgrading a monster in the encounter (giving it a lower rank) or by removing a monster.
        /// </summary>
        /// <param name="encounter">The encounter that should be made easier.</param>
        private void MakeEncounterEasier(EncounterDefinition encounter)
        {
            // There are two main ways to make an encounter easier - decrease of a rank or remove a monster. So first find if there are monsters we can downgrade.
            // We consider only elites - bosses should not be downgraded if possible and normal creatures cannot be downgraded.
            var eliteRankDownCandidates = encounter.AllEncounterGroups.Where(group => group.MonsterType.Rank == MonsterRank.Elite && CanDowngradeMonster(group.MonsterType)).ToArray();
            // Now find the monsters we can remove. Bosses and leaders are important and should not be removed unless absolutely necessary.
            var removeMonsterCandidates = encounter.AllEncounterGroups.Where(group => group.MonsterType.Rank != MonsterRank.Boss && group.MonsterType.Role != MonsterRole.Leader).ToArray();
            // We need to know how many monsters are there in an encounter, because if there is only one monster left, we do not remove it, we downgrade it if possible.
            var monsterCountInEncounters = encounter.AllEncounterGroups.Sum(group => group.MonsterCount);

            // It is apparent that if a monster can be downgraded, the same monster can also be removed.
            // If we can rank down a monster, we make it fifty/fifty whether we downgrade a monster or remove it.
            // If there is only one monster in the encounter, we force the downgrade, we will not remove it.
            if (eliteRankDownCandidates.Any() && (UnityEngine.Random.value < 0.5 || monsterCountInEncounters == 1))
            {
                DowngradeMonster(encounter, eliteRankDownCandidates.GetRandomElementOrDefault().MonsterType);
            }
            else if (removeMonsterCandidates.Any() && monsterCountInEncounters > 1)
            {
                // We did not downgrade, try removing.
                RemoveSpecificMonsterFromEncounter(encounter, removeMonsterCandidates.GetRandomElementOrDefault().MonsterType);
            }
            else
            {
                // The encounter is apparently only a leader and/or a boss. Welp, too bad. If there is a boss, downgrade it or remove it. If there is just a leader, well... Nothing to be done.
                // Single elite leader should be beatable by absolutely anyone.
                // So try to find a boss and downgrade/remove it.
                var boss = encounter.AllEncounterGroups.FirstOrDefault(group => group.MonsterType.Rank == MonsterRank.Boss);
                if (boss == null)
                {
                    return; // One creature in the encounter, nothing to be done.
                }
                if (CanDowngradeMonster(boss.MonsterType))
                {
                    DowngradeMonster(encounter, boss.MonsterType);
                }
                else if (monsterCountInEncounters > 1)
                {
                    RemoveSpecificMonsterFromEncounter(encounter, boss.MonsterType);
                }
            }
        }
        /// <summary>
        /// Lowers the rank of a specified monster.
        /// </summary>
        /// <param name="encounter">The encounter we are modifying. </param>
        /// <param name="toDowngrade">The monster we should downgrade.</param>
        private void DowngradeMonster(EncounterDefinition encounter, MonsterType toDowngrade)
        {
            RemoveSpecificMonsterFromEncounter(encounter, toDowngrade);
            var downgradedType = toDowngrade;
            downgradedType.Rank--;
            AddSpecificMonsterToEncounter(encounter, downgradedType);
        }
        /// <summary>
        /// Tries to insert a creature to the encounter which, if possible, will fit the requirements of the encounter type.
        /// </summary>
        /// <param name="encounter">The encounter we are modifying.</param>
        /// <param name="maxMonsterWeight">The maximum weight of the generated monster.</param>
        private void AddValidMonsterToEncounter(EncounterDefinition encounter, float maxMonsterWeight)
        {
            // First, figure out the attributes of the encounter we have so far related to the encounter type.
            var hasBoss = encounter.AllEncounterGroups.Any(group => group.MonsterType.Rank == MonsterRank.Boss);
            var hasLeader = encounter.AllEncounterGroups.Any(group => group.MonsterType.Role == MonsterRole.Leader);
            var currentOffenseDefenseRatio = encounter.GetAttackDefenseRatio(configuration);
            // Now we start filtering the list of possible monster types we could add by the criteria from the requested encounter type.
            // If the filters are too restrictive and we would return no monsters, we reset the candidates to the state before the filter was applied.
            // So the filters here are ordered from the most important ones to the least important ones.
            var candidates = new List<MonsterType>(availableMonsterTypes);
            candidates = FilterCandidatesResettingIfEmpty(candidates, candidate => configuration.MonsterRankWeights[candidate] <= maxMonsterWeight);

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
            // TODO: Bigger probabilities for those close to what we want.
            var selectedMonster = candidates.GetRandomElementOrDefault();
            AddSpecificMonsterToEncounter(encounter, selectedMonster);   
        }
        /// <summary>
        /// Removes the monster of a specified type from the encounter.
        /// </summary>
        /// <param name="encounter">The encounter we are modifying.</param>
        /// <param name="monsterToRemove">The monster we are removing from the encounter.</param>
        private void RemoveSpecificMonsterFromEncounter(EncounterDefinition encounter, MonsterType monsterToRemove)
        {
            for (int i = 0; i < encounter.AllEncounterGroups.Count; ++i)
            {
                if (encounter.AllEncounterGroups[i].MonsterType == monsterToRemove)
                {
                    encounter.AllEncounterGroups[i].MonsterCount--;
                    if (encounter.AllEncounterGroups[i].MonsterCount <= 0)
                    {
                        encounter.AllEncounterGroups.RemoveAt(i);
                        break;
                    }
                }
            }
        }
        /// <summary>
        /// Inserts the specified monster to the encounter.
        /// </summary>
        /// <param name="encounter">Encounter to modify.</param>
        /// <param name="monsterToAdd">Monster to add to the encounter.</param>
        private void AddSpecificMonsterToEncounter(EncounterDefinition encounter, MonsterType monsterToAdd)
        {
            var relevantGroup = encounter.AllEncounterGroups.FirstOrDefault(group => group.MonsterType == monsterToAdd);
            if (relevantGroup == null)
            {
                relevantGroup = new MonsterGroup { MonsterType = monsterToAdd };
                encounter.AllEncounterGroups.Add(relevantGroup);
            }
            var selectedMonsterWeight = configuration.MonsterRankWeights[monsterToAdd];
            // If adding minions (or any other monster with below one weight), add it enough times that it is one standard monster
            relevantGroup.MonsterCount++;
        }
        /// <summary>
        /// Filters the given list by some predicate. If the predicate would result in the list being empty, return the original list.
        /// </summary>
        /// <param name="candidates">The list of candidates to filter.</param>
        /// <param name="predicate">Predicate to filter the monsters.</param>
        /// <returns>The list of candidates with an applied filter, or the original list of candidates if the predicate removed every single monster.</returns>
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
        /// This checks whether there is a monster of that rank in the list of allowed monsters. 
        /// </summary>
        /// <param name="toDowngrade">Monster type to downgrade</param>
        /// <returns></returns>
        private bool CanDowngradeMonster(MonsterType toDowngrade)
        {
            var newRank = toDowngrade.Rank - 1;
            return availableMonsterTypes.Any(monsterType => monsterType.Rank == newRank && monsterType.Role == toDowngrade.Role);
        }
    }
}