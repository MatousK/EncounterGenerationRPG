using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.EncounterGenerator.Algorithm;
using Assets.Scripts.EncounterGenerator.Configuration;
using Assets.Scripts.EncounterGenerator.Model;
using Assets.Scripts.EncounterGenerator.Utils;
using UnityEngine;

namespace Assets.Scripts.EncounterGenerator
{
    public class EncounterGenerator
    {
        public EncounterGenerator()
        {
            encounterTypeManager = new EncounterTypeManager(generatorConfig);
        }

        private readonly EncounterGeneratorConfiguration generatorConfig = new EncounterGeneratorConfiguration();
        private readonly RandomWithHistory<MonsterGroupDefinition> monsterGroupRandom = new RandomWithHistory<MonsterGroupDefinition>();
        private readonly EncounterTypeManager encounterTypeManager;
        private readonly EncounterDifficultyMatrix difficultyMatrix = new EncounterDifficultyMatrix();
        private readonly MonstersManager monstersManager = new MonstersManager();

        public List<GameObject> GenerateEncounters(EncounterConfiguration configuration, PartyDefinition party)
        {
            if (!configuration.MonsterGroupDefinitions.Any())
            {
                // No monster definitions, so probably no monsters should spawn here.
                return new List<GameObject>();
            }
            var monsterGroupDefinition = monsterGroupRandom.RandomElementFromSequence(configuration.MonsterGroupDefinitions);
            var encounterGenerationAlgorithm = GetAlgorithm(configuration, party, monsterGroupDefinition);
            var encounterDefinition = encounterGenerationAlgorithm.GetEncounter();
            return monstersManager.GenerateMonsters(encounterDefinition, monsterGroupDefinition);
        }

        private EncounterGeneratorAlgorithm GetAlgorithm(EncounterConfiguration configuration, PartyDefinition party, MonsterGroupDefinition monsterGroupDefinition)
        {
            return new EncounterGeneratorAlgorithm
            {
                AvailableMonsterTypes = monsterGroupDefinition.GetAvailableMonsterTypes(),
                Party = party,
                Configuration = generatorConfig,
                Difficulty = configuration.EncounterDifficulty,
                EncounterTypeManager = encounterTypeManager,
                DifficultyMatrix = difficultyMatrix,
            };
        }
    }
}