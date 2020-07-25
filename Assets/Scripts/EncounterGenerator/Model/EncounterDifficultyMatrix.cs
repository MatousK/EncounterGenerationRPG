using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.EncounterGenerator.Configuration;
using UnityEngine;

namespace Assets.Scripts.EncounterGenerator.Model
{
    /// <summary>
    /// Class containing information about how all the different encounters would end.
    /// Can also calculate a difficulty of encounters not in the matrix.
    /// </summary>
    public class EncounterDifficultyMatrix
    {
        /// <summary>
        /// Used while estimating a difficulty for an encounter.
        /// Represents a candidate for some matrix element which could be the closest one to the requested encounter.
        /// </summary>
        class DifficultyCandidate
        {
            /// <summary>
            /// How many resources were lost in the encounter, i.e. difficulty, i.e. max hp lost.
            /// </summary>
            public float ResourcesLost;
            /// <summary>
            /// How close in the matrix is this element to the evaluated encounter. The SMALLER the better.
            /// </summary>
            public int CandidateWeight;
        }
        /// <summary>
        /// List of all elements currently in the matrix.
        /// </summary>
        public List<EncounterDifficultyMatrixElement> MatrixElements = new List<EncounterDifficultyMatrixElement>();
        /// <summary>
        /// Creates a deep clone of this matrix.
        /// </summary>
        /// <returns>The deep clone of this matrix.</returns>
        public EncounterDifficultyMatrix Clone()
        {
            return new EncounterDifficultyMatrix
            {
                MatrixElements = new List<EncounterDifficultyMatrixElement>(MatrixElements.Select(
                    originalElement => new EncounterDifficultyMatrixElement(originalElement.ElementSource)
                    ))
            };
        }
        /// <summary>
        /// Estimates the difficulty for some encounter.
        /// </summary>
        /// <param name="encounter">The enemies in this encounter.</param>
        /// <param name="party">The player characters fighting in this encounter.</param>
        /// <param name="configuration">The general configuration for the pathfinding algorithm.</param>
        /// <returns>The estimated difficulty of the encounter.</returns>
        public float GetDifficultyFor(EncounterDefinition encounter, PartyDefinition party, EncounterGeneratorConfiguration configuration)
        {
            // We compute the power of the monsters and the player, as they will be used over and over.
            encounter.UpdatePrecomputedMonsterCount(configuration);
            float partyPower = party.GetPartyStrength();
            // This array should always contain the 5 closest candidate. Whenever we process a new element, we add it do the last position and then sort the array.
            // This puts the worst element at the end of the array.
            DifficultyCandidate[] candidates = new DifficultyCandidate[6];
            for (int i = 0; i < candidates.Length; i++)
            {
                // Initialize with max value so they will be always replaced during the  followinfalgorithm.
                candidates[i] = new DifficultyCandidate { CandidateWeight = int.MaxValue };
            }
            foreach (var element in MatrixElements)
            {
                // Calculate how different is the encounter from the parameters from the encounter being iterated.
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
        /// <summary>
        /// Retrieve the encounter from the matrix with the most similar party and difficulty to the ones passed as parameters.
        /// </summary>
        /// <param name="party">The party for which we want the example encounter.</param>
        /// <param name="desiredResourcesLost">How many resources we want the player to lose in the encounter. So Max HP percentage lost, i.e. the difficulty.</param>
        /// <returns>Some encounter of similar difficulty and party.</returns>
        public EncounterDifficultyMatrixElement GetExampleEncounter(PartyDefinition party, float desiredResourcesLost)
        {
            float partyPower = party.GetPartyStrength();
            // Find an encounter with the lowest possible difference
            float currentMinResult = float.PositiveInfinity;
            EncounterDifficultyMatrixElement currentBestCandidate = null;
            // Go through all elements, calculate some sort of score for them, how similar they are to the party and difficulty being requested and fight the best one.
            foreach (var element in MatrixElements)
            {
                // Indexes of neighbouring buckets still differ by a thousands, see the GetPartyPowerBucket method. So it is much more important to match the correct party, we then
                // in the correct party for those with the most similar resources lost.
                var cost = Math.Abs(GetPartyPowerBucket(element.PartyPower) - GetPartyPowerBucket(partyPower)) + Math.Abs(desiredResourcesLost - element.ResourcesLost);
                if (cost < currentMinResult)
                {
                    currentMinResult = cost;
                    currentBestCandidate = element;
                }
            }
            return currentBestCandidate;
        }

        /// <summary>
        /// Group party power into buckets, as party power e.g. 4500 and 4300 are actually pretty much the same in game terms.
        /// </summary>
        /// <param name="partyPower">The party power to map.</param>
        /// <returns>Party power mapped onto some number.</returns>
        private int GetPartyPowerBucket(float partyPower)
        {
            // TODO: Figure out some proper value for the constants.
            // Party power is in buckets by 2000. But we also multiply it by 1000 to have it in the same order of magnitude as monster strengths. 
            return (int)(Math.Round(partyPower / 2000) * 1000);
        }
    }
    /// <summary>
    /// A single element in the difficulty matrix.
    /// </summary>
    public class EncounterDifficultyMatrixElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EncounterDifficultyMatrixElement"/> based on a line from a source file loaded when the game starts.
        /// </summary>
        /// <param name="elementSource"></param>
        public EncounterDifficultyMatrixElement(DifficultyMatrixSourceLine elementSource)
        {
            ElementSource = elementSource;
            PartyPower = elementSource.PartyStrength;
            ResourcesLost = elementSource.MaxHpLost;
            EncounterGroups = elementSource.EncounterDefinition;
        }


        /// <summary>
        /// The y axis, i.e. the encounter this element represents.
        /// </summary>
        public EncounterDefinition EncounterGroups;
        /// <summary>
        /// The x axis, i.e. how powerful was the party when fighting this encounter.
        /// </summary>
        public float PartyPower;
        /// <summary>
        /// The value, i.e. How many permanent resources did the party lose.
        /// </summary>
        public float ResourcesLost;
        /// <summary>
        /// All details of the fight in the difficulty matrix, mainly for debugging. The other values are sufficient for the algorithm.
        /// But this value is used when cloning the matrix, as we simply create a new matrix element with the same source.
        /// </summary>
        public DifficultyMatrixSourceLine ElementSource;
    }
}
