using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Combat;
using Assets.Scripts.EncounterGenerator.Configuration;
using UnityEngine;

namespace Assets.Scripts.EncounterGenerator.Model
{
    /// <summary>
    /// Represents the types of all monsters that should spawn in an encounter.
    /// </summary>
    [Serializable]
    public class EncounterDefinition
    {

        /// <summary>
        /// All of the monsters that should spawn in the encounter.
        /// </summary>
        public List<MonsterGroup> AllEncounterGroups;
        /// <summary>
        /// Sum of weights of all monsters in the encounter. Calculated often, so we precompute it.
        /// </summary>
        private float? precomputedAdjustedMonsterCount;
        /// <summary>
        /// Creates a new instance of the <see cref="EncounterDefinition"/> class. This encounter definition will represent monster types of <paramref name="monsterObjects"/>.
        /// </summary>
        /// <param name="monsterObjects">The monsters the new encounter definition should represent.</param>
        /// <returns>The encounter representing the monsters.</returns>
        public static EncounterDefinition GetDefinitionFromMonsters(List<GameObject> monsterObjects)
        {
            var monsters = monsterObjects.Select(mo => mo.GetComponent<Monster>()).Where(monster => monster != null)
                .ToArray();
            List<MonsterGroup> monsterGroups = new List<MonsterGroup>();
            foreach (var monster in monsters)
            {
                var currentMonsterType = new MonsterType(monster.Rank, monster.Role);
                var existingGroup = monsterGroups.FirstOrDefault(group => group.MonsterType == currentMonsterType);
                if (existingGroup != null)
                {
                    existingGroup.MonsterCount++;
                }
                else
                {
                    monsterGroups.Add(new MonsterGroup(currentMonsterType, 1));
                }
            }

            return new EncounterDefinition
            {
                AllEncounterGroups = monsterGroups
            };
        }
        /// <summary>
        /// Creates a deep clone of this class.
        /// </summary>
        /// <returns>A deep clone of this class.</returns>
        public EncounterDefinition Clone()
        {
            return new EncounterDefinition
            {
                AllEncounterGroups = new List<MonsterGroup>(AllEncounterGroups.Select(group => group.Clone())),
                precomputedAdjustedMonsterCount = precomputedAdjustedMonsterCount
            };
        }
        /// <summary>
        /// Updates the precomputed monster count. Call after modifying the encounter before passing it to the encounter generator.
        /// </summary>
        /// <param name="configuration">The general algorithm configuration for the encounter generator.</param>
        public void UpdatePrecomputedMonsterCount(EncounterGeneratorConfiguration configuration)
        {
            precomputedAdjustedMonsterCount = AllEncounterGroups.Sum(group => group.GetAdjustedMonsterCount(configuration));
        }
        /// <summary>
        /// Retrieve <see cref="precomputedAdjustedMonsterCount"/>, i.e. the sum of weights of all monsters.
        /// If the value was not precomputed, generate it, save the new value and return it.
        /// </summary>
        /// <param name="configuration">The general algorithm configuration for the encounter generator.</param>
        /// <returns>The adjusted monster count, i.e. the sum of weights of all monsters in the encounter.</returns>
        public float GetAdjustedMonsterCount(EncounterGeneratorConfiguration configuration)
        {
            if (precomputedAdjustedMonsterCount == null)
            {
                UpdatePrecomputedMonsterCount(configuration);
            }

            return precomputedAdjustedMonsterCount.Value;
        }
        /// <summary>
        /// Retrieve the average attack/defense ratio of monsters in this encounter.
        /// </summary>
        /// <param name="configuration">The general algorithm configuration for the encounter generator.</param>
        /// <returns>The current attack/defense ratio of this encounter.</returns>
        public float GetAttackDefenseRatio(EncounterGeneratorConfiguration configuration)
        {
            var attackDefenseSum = AllEncounterGroups.Sum(group =>
                configuration.MonsterRoleAttackDefenseRatios[group.MonsterType.Role] *
                group.GetAdjustedMonsterCount(configuration));
            return attackDefenseSum / GetAdjustedMonsterCount(configuration);
        }
        /// <summary>
        /// Retrieve how similar is this encounter to another encounter. The smaller the positive number, the better.
        /// </summary>
        /// <param name="other">The encounter to compare this encounter with.</param>
        /// <param name="configuration">The general algorithm configuration for the encounter generator.</param>
        /// <returns>The difference between this and <paramref name="other"/> encounter.</returns>
        public float GetDistance(EncounterDefinition other, EncounterGeneratorConfiguration configuration)
        {
            // TODO: Come up with a better distance algorithm.
            var otherAdjustedMonsterCount = other.GetAdjustedMonsterCount(configuration);
            var thisAdjustedMonsterCount = GetAdjustedMonsterCount(configuration);
            return Math.Abs(otherAdjustedMonsterCount - thisAdjustedMonsterCount);
        }
    }
}