using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.EncounterGenerator.Algorithm;
using Assets.Scripts.EncounterGenerator.Configuration;
using Assets.Scripts.EncounterGenerator.Model;
using Assets.Scripts.EncounterGenerator.Utils;
using UnityEngine;

namespace Assets.Scripts.EncounterGenerator.Development
{
    public class StaticEncounterGenerator: MonoBehaviour
    {
        public EncounterDefinition StaticEncounter;
        private readonly MonstersManager monstersManager = new MonstersManager();
        private readonly RandomWithHistory<MonsterGroupDefinition> monsterGroupRandom = new RandomWithHistory<MonsterGroupDefinition>();

        public List<GameObject> GenerateEncounters(EncounterConfiguration configuration)
        {
            if (!configuration.MonsterGroupDefinitions.Any())
            {
                // No monster definitions, so probably no monsters should spawn here.
                return new List<GameObject>();
            }
            var monsterGroupDefinition = monsterGroupRandom.RandomElementFromSequence(configuration.MonsterGroupDefinitions);
            return monstersManager.GenerateMonsters(StaticEncounter, monsterGroupDefinition);
        }
    }
}