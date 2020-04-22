using System;
using System.Collections.Generic;
using Assets.Scripts.EncounterGenerator.Configuration;
using Assets.Scripts.EncounterGenerator.Model;

namespace Assets.Scripts.EncounterGenerator.Algorithm
{
    public class EncounterGeneratorAlgorithm
    {
        public EncounterMatrixUpdater MatrixUpdater;
        public EncounterTypeManager EncounterTypeManager;
        public List<MonsterType> AvailableMonsterTypes;
        public EncounterGeneratorConfiguration Configuration;
        public EncounterDifficulty Difficulty;
        public EncounterDifficultyMatrix DifficultyMatrix;
        public PartyDefinition Party;
        private float targetDifficulty;
        private const float MaxDifficultyDifference = 0.1f;
        private const int MaxIterations = 20;

        public EncounterDefinition GetEncounter()
        {
            // First we select an encounter type - that is just a very high level overview of what kind of combat this will be.
            // This is not in any way related to difficulty, an encounter of some type would be applicable to all difficulties.
            var encounterType = EncounterTypeManager.SelectEncounterType(AvailableMonsterTypes);
            // This class can generate specific encounters based on the type given in constructer.
            var encounterDefinitionManager = new EncounterDefinitionManager(Configuration, AvailableMonsterTypes, encounterType);
            targetDifficulty = Difficulty.GetDifficultyForParty(Party);
            // We get some encounter reasonably close to what we are asking for right now.
            var exampleEncounter = DifficultyMatrix.GetExampleEncounter(Party, targetDifficulty);
            // And turn it into an encounter of the proper type;
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
            MatrixUpdater.StoreCombatStartConditions(Party, closestCandidate, closestCandidateDifficulty);
            return closestCandidate;
        }
    }
}
