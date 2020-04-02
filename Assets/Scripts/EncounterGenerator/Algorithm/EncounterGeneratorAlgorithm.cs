using EncounterGenerator.Configuration;
using EncounterGenerator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EncounterGenerator.Algorithm
{
    class EncounterGeneratorAlgorithm
    {
        public EncounterTypeManager EncounterTypeManager;
        public List<MonsterType> AvailableMonsterTypes;
        public EncounterGeneratorConfiguration Configuration;
        public EncounterDifficulty Difficulty;
        public EncounterDifficultyMatrix DifficultyMatrix;
        public PartyDefinition Party;
        float TargetDifficulty;
        const float MaxDifficultyDifference = 10;
        const int MaxIterations = 10;

        public EncounterDefinition GetEncounter()
        {
            // First we select an encounter type - that is just a very high level overview of what kind of combat this will be.
            // This is not in any way related to difficulty, an encounter of some type would be applicable to all difficulties.
            var encounterType = EncounterTypeManager.SelectEncounterType(AvailableMonsterTypes);
            // This class can generate specific encounters based on the type given in constructer.
            var encounterDefinitionManager = new EncounterDefinitionManager(Configuration, AvailableMonsterTypes, encounterType);
            TargetDifficulty = Difficulty.GetDifficultyForParty(Party);
            // We get some encounter reasonably close to what we are asking for right now.
            var exampleEncounter = DifficultyMatrix.GetExampleEncounter(Party, TargetDifficulty);
            // And turn it into an encounter of the proper type;
            EncounterDefinition candidate = encounterDefinitionManager.GenerateEncounter(exampleEncounter.EncounterGroups);
            float candidateDifficulty = DifficultyMatrix.GetDifficultyFor(candidate, Party, Configuration);

            // We might not always be able to generate an encounter of the proper difficulty.
            // We do multiple iterations and look for the candidate as close to the correct difficulty as possible.
            EncounterDefinition closestCandidate = candidate.Clone();
            float closestDifficultyDifference = Math.Abs(TargetDifficulty - candidateDifficulty);
            int currentIteration = 0;

            while (closestDifficultyDifference > MaxDifficultyDifference && ++currentIteration <= MaxIterations)
            {
                // Last generated encounter was not good enough. Make it easier/harder and evaluate it again.
                encounterDefinitionManager.AdjustEncounter(candidate, candidateDifficulty < TargetDifficulty);
                candidateDifficulty = DifficultyMatrix.GetDifficultyFor(candidate, Party, Configuration);
                var candidateDifficultyDifference = Math.Abs(TargetDifficulty - candidateDifficulty);

                if (candidateDifficultyDifference < closestDifficultyDifference)
                {
                    // Keep track of the best possible result in case we won't manage to hit the difficulty precisely.
                    closestDifficultyDifference = candidateDifficultyDifference;
                    closestCandidate = candidate.Clone();
                }
            }

            return closestCandidate;
        }
    }
}
