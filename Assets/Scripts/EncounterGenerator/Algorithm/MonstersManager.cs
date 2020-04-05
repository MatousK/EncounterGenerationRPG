using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.EncounterGenerator.Configuration;
using Assets.Scripts.EncounterGenerator.Model;
using UnityEngine;

namespace Assets.Scripts.EncounterGenerator.Algorithm
{
    public class MonstersManager
    {
        private readonly Dictionary<GameObject, float> monsterPriorities = new Dictionary<GameObject, float>();

        public List<GameObject> GenerateMonsters(EncounterDefinition encounterDefinition, MonsterGroupDefinition monsterGroupDefinition)
        {
            var parameters = new GenerateMonsterGroupParameters
            {
                RequestedMonsters = encounterDefinition,
                MonsterPriorities = monsterPriorities
            };
            var generatedMonsters = monsterGroupDefinition.GenerateMonsterGroup(parameters);
            // Raise the chance of generating monsters that were not generated right now and reset chances of those already generated.
            foreach (var existingMonster in monsterPriorities.Keys.ToList())
            {
                monsterPriorities[existingMonster]++;
            }
            foreach (var monster in generatedMonsters)
            {
                monsterPriorities[monster] = 0;
            }
            return generatedMonsters;
        }
    }
}
