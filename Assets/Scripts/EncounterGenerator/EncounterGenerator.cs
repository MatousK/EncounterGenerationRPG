using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.EncounterGenerator.Algorithm;
using Assets.Scripts.EncounterGenerator.Configuration;
using Assets.Scripts.EncounterGenerator.Model;
using Assets.Scripts.EncounterGenerator.Utils;
using UnityEngine;

namespace Assets.Scripts.EncounterGenerator
{
    /// <summary>
    /// The class responsible for generating encounters.
    /// </summary>
    public class EncounterGenerator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EncounterGenerator"/> class.
        /// </summary>
        /// <param name="difficultyMatrix">Difficulty matrix used to estimate difficulty.</param>
        /// <param name="matrixUpdater">The object which can update the matrix once the combat is over.</param>
        /// <param name="generatorConfig">The general configuration for the encounter generator.</param>
        public EncounterGenerator(EncounterDifficultyMatrix difficultyMatrix, EncounterMatrixUpdater matrixUpdater, EncounterGeneratorConfiguration generatorConfig)
        {
            encounterTypeManager = new EncounterTypeManager(generatorConfig);
            this.difficultyMatrix = difficultyMatrix;
            this.matrixUpdater = matrixUpdater;
            this.generatorConfig = generatorConfig;
        }
        /// <summary>
        /// The general configuration for the encounter generator.
        /// </summary>
        private readonly EncounterGeneratorConfiguration generatorConfig;
        /// <summary>
        /// Selects a monster group for an encounter. Has history, so it tries not to select the encounter it has selected recently.
        /// </summary>
        private readonly RandomWithHistory<MonsterGroupDefinition> monsterGroupRandom = new RandomWithHistory<MonsterGroupDefinition>();
        /// <summary>
        /// The object which can select the encounter type for the encounter that will be generated. 
        /// </summary>
        private readonly EncounterTypeManager encounterTypeManager;
        /// <summary>
        /// The difficulty matrix used to estimate difficulty of encounters.
        /// </summary>
        private readonly EncounterDifficultyMatrix difficultyMatrix;
        /// <summary>
        /// The object which can generate monsters for an encounter. Has history, so it can try to generate monsters it has not generated in some time.
        /// </summary>
        private readonly MonstersManager monstersManager = new MonstersManager();
        /// <summary>
        /// The object which will update the matrix after every encounter. We need to set the start conditions in it.
        /// </summary>
        private readonly EncounterMatrixUpdater matrixUpdater;
        /// <summary>
        /// Generates a new generator for some configuration.
        /// </summary>
        /// <param name="configuration">Configuration of one single encounter that should be generated, i.e. which monsters can appear there and how difficult should the encounter be.</param>
        /// <param name="party">The party who will fight in this encounter.</param>
        /// <returns>The generated encounter that should be fought.</returns>
        public List<GameObject> GenerateEncounters(EncounterConfiguration configuration, PartyDefinition party)
        {
            if (!configuration.MonsterGroupDefinitions.Any())
            {
                // No monster definitions, so probably no monsters should spawn here.
                return new List<GameObject>();
            }
            // Select which monster group we will use, generate an encounter definition of monster types that should appear and then select specific monsters for that encounter definition.
            var monsterGroupDefinition = monsterGroupRandom.RandomElementFromSequence(configuration.MonsterGroupDefinitions);
            var encounterGenerationAlgorithm = GetAlgorithm(configuration, party, monsterGroupDefinition);
            var encounterDefinition = encounterGenerationAlgorithm.GetEncounter();
            return monstersManager.GenerateMonsters(encounterDefinition, monsterGroupDefinition);
        }
        /// <summary>
        /// Creates the algorithm object initialized with the parameters given and the fields of this class.
        /// </summary>
        /// <param name="configuration">Configuration of the single encounter that should be generated.</param>
        /// <param name="party">The party that will fight in this encounter.</param>
        /// <param name="monsterGroupDefinition">Which monsters should appear in this encounter.</param>
        /// <returns>Algorithm object that is ready to produce the encounter.</returns>
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
                MatrixUpdater = matrixUpdater,
            };
        }
    }
}