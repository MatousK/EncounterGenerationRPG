using EncounterGenerator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EncounterGenerator.Algorithm
{
    public class MonstersManager
    {
        private Dictionary<GameObject, float> MonsterPriorities = new Dictionary<GameObject, float>();

        public List<GameObject> GenerateMonsters(EncounterDefinition encounterDefinition, MonsterGroupDefinition monsterGroupDefinition)
        {
            var parameters = new GenerateMonsterGroupParameters
            {
                RequestedMonsters = encounterDefinition,
                MonsterPriorities = MonsterPriorities
            };
            var generatedMonsters = monsterGroupDefinition.GenerateMonsterGroup(parameters);
            // Raise the chance of generating monsters that were not generated right now and reset chances of those already generated.
            foreach (var existingMonster in MonsterPriorities.Keys.ToList())
            {
                MonsterPriorities[existingMonster]++;
            }
            foreach (var monster in generatedMonsters)
            {
                MonsterPriorities[monster] = 0;
            }
            return generatedMonsters;
        }
    }
}
