using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.EncounterGenerator.Configuration;
using UnityEngine;

namespace Assets.Scripts.EncounterGenerator.Model
{
    public class EncounterDifficultyMatrix
    {
        class DifficultyCandidate
        {
            public float ResourcesLost;
            public int CandidateWeight;
        }
        // TODO: Algorithms here are quite inefficient for now, but, well, we must first find out if it works at all.
        public List<EncounterDifficultyMatrixElement> MatrixElements = new List<EncounterDifficultyMatrixElement>();

        public EncounterDifficultyMatrix Clone()
        {
            return new EncounterDifficultyMatrix
            {
                MatrixElements = new List<EncounterDifficultyMatrixElement>(MatrixElements.Select(
                    originalElement => new EncounterDifficultyMatrixElement(originalElement.ElementSource)
                    ))
            };
        }

        public float GetDifficultyFor(EncounterDefinition encounter, PartyDefinition party, EncounterGeneratorConfiguration configuration)
        {
            encounter.UpdatePrecomputedMonsterCount(configuration);
            float partyPower = party.GetPartyStrength();
            // TODO: We need to check this thoroughly to figure out which is more important - closeness in encounter type or closeness in party power. For now we treat them 50/50.
            DifficultyCandidate[] candidates = new DifficultyCandidate[6];
            for (int i = 0; i < candidates.Length; i++)
            {
                candidates[i] = new DifficultyCandidate { CandidateWeight = int.MaxValue };
            }
            foreach (var element in MatrixElements)
            {
                var distance = element.EncounterGroups.GetDistance(encounter, configuration);
                var powerDifference = Math.Abs(GetPartyPowerBucket(partyPower) - GetPartyPowerBucket(element.PartyPower));
                // The last element in the collection is always garbage, either worse than all the others just noninitialized element.
                // So put the current candidate there.
                var candidate = candidates[candidates.Length - 1];
                candidate.ResourcesLost = element.ResourcesLost;
                candidate.CandidateWeight = (int)(distance + powerDifference);
                // Sort all elements, the last one will be leftover and will be replaced later.
                Array.Sort(candidates, (x, y) => x.CandidateWeight - y.CandidateWeight);
            }
            // Drop the last element, thereby actually gaining the 5 best candidates.
            var actualCandidates = candidates.Take(5).ToArray();
            if (actualCandidates.Any(candidate => candidate.CandidateWeight == 0))
            {
                // We found some encounters that fit perfectly - just average them.
                return actualCandidates.Where(candidate => candidate.CandidateWeight == 0)
                    .Average(candidate => candidate.ResourcesLost);
            }
            float totalWeight = actualCandidates.Sum(candidate => candidate.CandidateWeight);
            // Return weighted average of five closest candidates.
            return actualCandidates.Sum(candidate => candidate.ResourcesLost * (candidate.CandidateWeight / totalWeight));
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
                var cost = Math.Abs(GetPartyPowerBucket(element.PartyPower) - GetPartyPowerBucket(partyPower)) * 10 + Math.Abs(desiredResourcesLost - element.ResourcesLost);
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
            // Party power is in buckets by 2000. But we also multiply it by 100 to have it in the same order of magnitude as monster strengths. 
            return (int)(Math.Round(partyPower / 2000) * 1000);
        }
    }

    public class EncounterDifficultyMatrixElement
    {
        public EncounterDifficultyMatrixElement(DifficultyMatrixSourceLine elementSource)
        {
            ElementSource = elementSource;
            PartyPower = elementSource.PartyStrength;
            ResourcesLost = elementSource.MaxHpLost;
            EncounterGroups = elementSource.EncounterDefinition;
        }


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
        /// <summary>
        /// All details of the fight in the difficulty matrix, mainly for debugging.
        /// </summary>
        public DifficultyMatrixSourceLine ElementSource;
    }
}
