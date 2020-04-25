using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Combat;
using Assets.Scripts.EncounterGenerator.Configuration;
using UnityEngine;

namespace Assets.Scripts.EncounterGenerator.Model
{
    /// <summary>
    /// Represents all monsters that should spawn in an encounter.
    /// </summary>
    [Serializable]
    public class EncounterDefinition
    {

        /// <summary>
        /// All of the monsters that should spawn in the 
        /// </summary>
        public List<MonsterGroup> AllEncounterGroups;

        private float? precomputedAdjustedMonsterCount;

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

        public EncounterDefinition Clone()
        {
            return new EncounterDefinition
            {
                AllEncounterGroups = new List<MonsterGroup>(AllEncounterGroups.Select(group => group.Clone())),
                precomputedAdjustedMonsterCount = precomputedAdjustedMonsterCount
            };
        }

        public void UpdatePrecomputedMonsterCount(EncounterGeneratorConfiguration configuration)
        {
            precomputedAdjustedMonsterCount = AllEncounterGroups.Sum(group => group.GetAdjustedMonsterCount(configuration));
        }

        public float GetAdjustedMonsterCount(EncounterGeneratorConfiguration configuration)
        {
            if (precomputedAdjustedMonsterCount == null)
            {
                UpdatePrecomputedMonsterCount(configuration);
            }

            return precomputedAdjustedMonsterCount.Value;
        }

        public float GetAttackDefenseRatio(EncounterGeneratorConfiguration configuration)
        {
            var attackDefenseSum = AllEncounterGroups.Sum(group =>
                configuration.MonsterRoleAttackDefenseRatios[group.MonsterType.Role] *
                group.GetAdjustedMonsterCount(configuration));
            return attackDefenseSum / GetAdjustedMonsterCount(configuration);
        }

        public float GetDistance(EncounterDefinition other, EncounterGeneratorConfiguration configuration)
        {
            // TODO: Come up with a better distance algorithm.
            var otherAdjustedMonsterCount = other.GetAdjustedMonsterCount(configuration);
            var thisAdjustedMonsterCount = GetAdjustedMonsterCount(configuration);
            return Math.Abs(otherAdjustedMonsterCount - thisAdjustedMonsterCount);
        }
    }
}