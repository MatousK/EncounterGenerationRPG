using EncounterGenerator.Configuration;
using EncounterGenerator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncounterGenerator.Algorithm
{
    class EncounterGeneratorAlgorithm
    {

        public EncounterGeneratorConfiguration Configuration;
        public EncounterDifficulty Difficulty;
        public EncounterDifficultyMatrix DifficultyMatrix;
        public float PartyStrength;

        public EncounterDefinition GetEncounter()
        {
            var requestedDifficulty = Difficulty.GetDifficultyForPartyStrength(PartyStrength);

            var exampleEncounter = DifficultyMatrix.GetExampleEncounter(PartyStrength, requestedDifficulty);

            EncounterDefinition candidate = exampleEncounter.EncounterGroups;
            float candidateDifficulty = DifficultyMatrix.GetDifficultyFor(candidate, PartyStrength, Configuration);
            do
            {
                candidate = GetEncounterCandidate(candidate, PartyStrength, candidateDifficulty);
                candidateDifficulty = DifficultyMatrix.GetDifficultyFor(candidate, PartyStrength, Configuration);
            } while (!IsEncounterValid(candidate, candidateDifficulty));

            return candidate;
        }

        bool IsEncounterValid(EncounterDefinition encounter, float encounterDifficulty)
        {
            return true;
        }

        EncounterDefinition GetEncounterCandidate(EncounterDefinition previousCandidate, float candidatePartyStrength, float candidateDifficulty)
        {

        }
    }
}
