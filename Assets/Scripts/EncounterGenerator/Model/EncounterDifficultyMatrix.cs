using EncounterGenerator.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncounterGenerator.Model
{
    public class EncounterDifficultyMatrix
    {
        struct DifficultyCandidate
        {
            public float Distance;
            public float PowerDifference;
            public float ResourcesLost;

            public float GetCandidateWeight()
            {
                // TODO: Figure out constants;
                return PowerDifference + Distance;
            }
        }
        // TODO: Algorithms here are quite inefficient for now, but, well, we must first find out if it works at all.
        public List<EncounterDifficultyMatrixElement> MatrixElements = new List<EncounterDifficultyMatrixElement>();

        public float GetDifficultyFor(EncounterDefinition encounter, PartyDefinition party, EncounterGeneratorConfiguration configuration)
        {
            float partyPower = party.GetPartyStrength();
            // TODO: We need to check this thoroughly to figure out which is more important - closeness in encounter type or closeness in party power. For now we treat them 50/50.
            List<DifficultyCandidate> candidates = new List<DifficultyCandidate>(6);
            foreach (var element in MatrixElements)
            {
                var distance = element.EncounterGroups.GetDistance(encounter, configuration);
                var powerDifference = Math.Abs(GetPartyPowerBucket(partyPower) - GetPartyPowerBucket(element.PartyPower));
                // TODO: Figure out constants that actually make sense.
                if (distance < 1 && powerDifference == 0)
                {
                    // We found an element sufficiently close to the target.
                    return element.ResourcesLost;
                }
                candidates.Add(new DifficultyCandidate { Distance = distance, PowerDifference = powerDifference, ResourcesLost = element.ResourcesLost });
                // Sort all elements and drop the last one, effectively keeping a list of 5 items with lowest difference.
                candidates = candidates.OrderBy(candidate => candidate.GetCandidateWeight()).Take(5).ToList();
            }
            float totalWeight = candidates.Sum(candidate => candidate.GetCandidateWeight());
            // Return weighted average of five closest candidates.
            return candidates.Sum(candidate => candidate.ResourcesLost * (candidate.GetCandidateWeight() / totalWeight));
        }

        public EncounterDifficultyMatrixElement GetExampleEncounter(PartyDefinition party, float desiredResourcesLost)
        {
            float partyPower = party.GetPartyStrength();
            //TODO: Normalize units. If party power worked in thousands and resources lost in hundreds, this would not really work.
            // Find an encounter with the lowest possible difference
            float currentMinResult = float.PositiveInfinity;
            EncounterDifficultyMatrixElement currentBestCandidate = null;
            foreach (var element in MatrixElements)
            {
                var cost = Math.Abs(GetPartyPowerBucket(element.PartyPower) - GetPartyPowerBucket(partyPower))*10 + Math.Abs(desiredResourcesLost - element.ResourcesLost);
                if (cost < currentMinResult)
                {
                    currentMinResult = cost;
                    currentBestCandidate = element;
                }
            }
            return currentBestCandidate;
        }

        /// <summary>
        /// Party power is continous, but we want the matrix to be discrete. Therefore this functions maps the party power into a bucket.
        /// </summary>
        /// <param name="partyPower">The party power to map.</param>
        /// <returns>Party power mapped onto a single number.</returns>
        private int GetPartyPowerBucket(float partyPower)
        {
            // TODO: Figure out some proper value for the constants.
            return (int)(partyPower / 50);
        }
    }

    public class EncounterDifficultyMatrixElement
    {
        /// <summary>
        /// The x axis, i.e. the encounter this element represents.
        /// </summary>
        public EncounterDefinition EncounterGroups;
        /// <summary>
        /// The y axis, i.e. how powerful was the party when fighting this encounter.
        /// </summary>
        public float PartyPower;
        /// <summary>
        /// The value, i.e. How many permamnent resources did the party lose.
        /// </summary>
        public float ResourcesLost;
    }
}
