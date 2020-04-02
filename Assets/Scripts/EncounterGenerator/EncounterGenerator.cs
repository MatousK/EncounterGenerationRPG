using EncounterGenerator.Algorithm;
using EncounterGenerator.Configuration;
using EncounterGenerator.Model;
using EncounterGenerator.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EncounterGenerator
{
    public class EncounterGenerator
    {
        public EncounterGenerator()
        {
            EncounterTypeManager = new EncounterTypeManager(GeneratorConfig);
        }

        private readonly EncounterGeneratorConfiguration GeneratorConfig = new EncounterGeneratorConfiguration();
        private readonly RandomWithHistory<MonsterGroupDefinition> MonsterGroupRandom = new RandomWithHistory<MonsterGroupDefinition>();
        private readonly EncounterTypeManager EncounterTypeManager;
        private readonly EncounterDifficultyMatrix DifficultyMatrix = new EncounterDifficultyMatrix();
        private readonly MonstersManager MonstersManager = new MonstersManager();

        public List<GameObject> GenerateEncounters(EncounterConfiguration configuration, PartyDefinition party)
        {
            if (!configuration.MonsterGroupDefinitions.Any())
            {
                // No monster definitions, so probably no monsters should spawn here.
                return new List<GameObject>();
            }
            var monsterGroupDefinition = MonsterGroupRandom.RandomElementFromSequence(configuration.MonsterGroupDefinitions);
            var encounterGenerationAlgorithm = GetAlgorithm(configuration, party, monsterGroupDefinition);
            var encounterDefinition = encounterGenerationAlgorithm.GetEncounter();
            return MonstersManager.GenerateMonsters(encounterDefinition, monsterGroupDefinition);
        }

        private EncounterGeneratorAlgorithm GetAlgorithm(EncounterConfiguration configuration, PartyDefinition party, MonsterGroupDefinition monsterGroupDefinition)
        {
            return new EncounterGeneratorAlgorithm
            {
                AvailableMonsterTypes = monsterGroupDefinition.GetAvailableMonsterTypes(),
                Party = party,
                Configuration = GeneratorConfig,
                Difficulty = configuration.EncounterDifficulty,
                EncounterTypeManager = EncounterTypeManager,
                DifficultyMatrix = DifficultyMatrix,
            };
        }
    }
}