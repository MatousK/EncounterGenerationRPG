using System;
using System.Collections.Generic;
using Assets.Scripts.EncounterGenerator.Configuration;
using Assets.Scripts.EncounterGenerator.Model;

namespace Assets.Scripts.EncounterGenerator.Algorithm
{
    /// <summary>
    /// The algorithm that can generate an encounter if configured with proper arguments.
    /// </summary>
    public class EncounterGeneratorAlgorithm
    {
        /// <summary>
        /// The object which can update the matrix once the encounter is over.
        /// </summary>
        public EncounterMatrixUpdater MatrixUpdater;
        /// <summary>
        /// The object which provides this class with the type of encounter to generate, see <see cref="EncounterType"/>.
        /// </summary>
        public EncounterTypeManager EncounterTypeManager;
        /// <summary>
        /// The list of monster types we are allowed to generate.
        /// </summary>
        public List<MonsterType> AvailableMonsterTypes;
        /// <summary>
        /// The general configuration for this algorithm.
        /// </summary>
        public EncounterGeneratorConfiguration Configuration;
        /// <summary>
        /// Difficulty of the encounter we should generate.
        /// </summary>
        public EncounterDifficulty Difficulty;
        /// <summary>
        /// The matrix containing difficulty information about many encounters used to estimate difficulty of generated encounters.
        /// </summary>
        public EncounterDifficultyMatrix DifficultyMatrix;
        /// <summary>
        /// The party for which we are generating an encounter.
        /// </summary>
        public PartyDefinition Party;
        /// <summary>
        /// The target difficulty for the encounter we are generating.
        /// </summary>
        private float targetDifficulty;
        /// <summary>
        /// How much is the the difficulty of the encounter allowed to differ from the target.
        /// </summary>
        private const float MaxDifficultyDifference = 0.1f;
        /// <summary>
        /// How many adjustments can the hill climbing algorithm do before using the best found encounter.
        /// </summary>
        private const int MaxIterations = 20;

        /// <summary>
        /// Generates an encounter based on the configuration of this class.
        /// </summary>
        /// <returns></returns>
        public EncounterDefinition GetEncounter()
        {
            // First we select an encounter type - that is just a very high level overview of what kind of combat this will be.
            // This is not in any way related to difficulty, an encounter of some type would be applicable to all difficulties.
            var encounterType = EncounterTypeManager.SelectEncounterType(AvailableMonsterTypes);
            // We create the object capable of creating encounters of the specified type.
            var encounterDefinitionManager = new EncounterDefinitionManager(Configuration, AvailableMonsterTypes, encounterType);
            // Retrieve the numeric difficulty of the encounter.
            targetDifficulty = Difficulty.GetDifficultyForParty(Party);
            // We get some encounter reasonably close to what we are asking for right now.
            var exampleEncounter = DifficultyMatrix.GetExampleEncounter(Party, targetDifficulty);
            // And turn it into an encounter of the proper type and calculate its difficulty.
            EncounterDefinition candidate = encounterDefinitionManager.GenerateEncounter(exampleEncounter.EncounterGroups);
            float candidateDifficulty = DifficultyMatrix.GetDifficultyFor(candidate, Party, Configuration);

            // We might not always be able to generate an encounter of the proper difficulty.
            // We do multiple iterations and look for the candidate as close to the correct difficulty as possible.
            EncounterDefinition closestCandidate = candidate.Clone();
            float closestDifficultyDifference = Math.Abs(targetDifficulty - candidateDifficulty);
            float closestCandidateDifficulty = candidateDifficulty;
            int currentIteration = 0;

            while (closestDifficultyDifference > MaxDifficultyDifference && ++currentIteration <= MaxIterations)
            {
                // Last generated encounter was not good enough. Make it easier/harder and evaluate it again.
                encounterDefinitionManager.AdjustEncounter(candidate, candidateDifficulty < targetDifficulty);
                candidateDifficulty = DifficultyMatrix.GetDifficultyFor(candidate, Party, Configuration);
                var candidateDifficultyDifference = Math.Abs(targetDifficulty - candidateDifficulty);

                if (candidateDifficultyDifference < closestDifficultyDifference)
                {
                    // Keep track of the best possible result in case we won't manage to hit the difficulty precisely.
                    closestDifficultyDifference = candidateDifficultyDifference;
                    closestCandidateDifficulty = candidateDifficulty;
                    closestCandidate = candidate.Clone();
                }
            }
            // Store the conditions at the start of the encounter, as well as the difficulty estimate, in order to update the matrix once the combat is over.
            MatrixUpdater.StoreCombatStartConditions(Party, closestCandidate, closestCandidateDifficulty);
            return closestCandidate;
        }
    }
}
