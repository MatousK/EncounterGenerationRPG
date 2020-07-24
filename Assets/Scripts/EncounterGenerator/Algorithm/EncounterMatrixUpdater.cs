using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Analytics;
using Assets.Scripts.Combat;
using Assets.Scripts.EncounterGenerator.Configuration;
using Assets.Scripts.EncounterGenerator.Model;
using UnityEngine;

namespace Assets.Scripts.EncounterGenerator.Algorithm
{
    /// <summary>
    /// After a fight is over, this class should update the difficulty matrix reflecting the combat result.
    /// </summary>
    public class EncounterMatrixUpdater
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EncounterMatrixUpdater"/> class.
        /// </summary>
        /// <param name="difficultyMatrix">The difficulty matrix we are updating.</param>
        /// <param name="configuration">The general configuration for the encounter generator.</param>
        /// <param name="analyticsService">The object used to send analytics data to the backend.</param>
        public EncounterMatrixUpdater(EncounterDifficultyMatrix difficultyMatrix, EncounterGeneratorConfiguration configuration, AnalyticsService analyticsService)
        {
            this.analyticsService = analyticsService;
            this.difficultyMatrix = difficultyMatrix;
            this.configuration = configuration;
        }
        /// <summary>
        /// This event will be called when the matrix changes. Eventargs are details about the encounter that changed the matrix.
        /// </summary>
        public event EventHandler<MatrixChangedEventArgs> MatrixChanged;
        /// <summary>
        /// This flag controls whether current combat should update the matrix or not.
        /// </summary>
        public bool AdjustMatrixForNextFight = true;
        /// <summary>
        /// If true, the currently handled fight is a static encounter.
        /// </summary>
        public bool IsStaticEncounter;
        /// <summary>
        /// The general configuration for the encounter generator.
        /// </summary>
        private readonly EncounterGeneratorConfiguration configuration;
        /// <summary>
        /// The difficulty matrix we are updating.
        /// </summary>
        private readonly EncounterDifficultyMatrix difficultyMatrix;
        /// <summary>
        /// Max HP of the heroes at the start of the encounter.
        /// </summary>
        private readonly Dictionary<HeroProfession, float> initialMaxHp = new Dictionary<HeroProfession, float>();
        /// <summary>
        /// HP of the heroes at the start of the encounter.
        /// </summary>
        private readonly Dictionary<HeroProfession, float> initialHp = new Dictionary<HeroProfession, float>();
        /// <summary>
        /// Attack of the heroes at the start of the encounter.
        /// </summary>
        private readonly Dictionary<HeroProfession, float> partyDamageMultipliers = new Dictionary<HeroProfession, float>();
        /// <summary>
        /// If not null, an encounter is currently being fought and this is its expected difficulty as estimated by the matrix.
        /// </summary>
        private float? expectedDifficulty;
        /// <summary>
        /// The encounter that is being fought right now, or null if no encounter is active right now.
        /// </summary>
        private EncounterDefinition encounter;
        /// <summary>
        /// The object used to send analytics data to the backend.
        /// </summary>
        private AnalyticsService analyticsService;
        /// <summary>
        /// Stores the conditions at the start of the encounter. We will use these when the combat ends to update the matrix.
        /// </summary>
        /// <param name="party">The party engaged in the encounter.</param>
        /// <param name="encounter">The monsters the party is fighting.</param>
        /// <param name="expectedDifficulty">The expected difficulty of the encounter as estimated by the matrix.</param>
        public void StoreCombatStartConditions(IPartyDefinition party, EncounterDefinition encounter, float expectedDifficulty)
        {
            foreach (var heroProfession in party.GetHeroProfessions())
            {
                initialMaxHp[heroProfession] = party.GetMaxHpForHero(heroProfession);
                initialHp[heroProfession] = party.GetHpForHero(heroProfession);
                partyDamageMultipliers[heroProfession] = party.GetAttackForHero(heroProfession);
            }
            // When analyzing the data from the first run of the experiment, we saw that sometimes the initial conditions were being set to 0.
            // We do not know how it happened, but as a hotfix we simply skip evaluation of these encounters.
            // When creating results summaries we need to emulate the bug, hence the condition on the hotfix.
            if (initialMaxHp.Any(p => p.Value == 0) && !configuration.EmulateV1Bug)
            {
                UnityEngine.Debug.LogError("Invalid matrix start data");
                return;
            }
            this.expectedDifficulty = expectedDifficulty;
            this.encounter = encounter;
        }
        /// <summary>
        /// Called when the combat ends to adjust the matrix and log the result to analytics.
        /// </summary>
        /// <param name="party">The party that just finished the encounter.</param>
        /// <param name="wasGameOver">If true, the party was defeated.</param>
        public void CombatOverAdjustMatrix(IPartyDefinition party, bool wasGameOver)
        {
            if (expectedDifficulty == null || !initialMaxHp.Any())
            {
                // Probably combat simulator, in release mode this would be a bug.
                UnityEngine.Debug.LogWarning("Logging combat encounter when initial conditions are not set");
                return; ;
            }
            // Again workaround for the V bug, see the comments in the StoreCombatStartConditions.
            if (initialMaxHp.Any(p => p.Value == 0) && !configuration.EmulateV1Bug)
            {
                UnityEngine.Debug.LogError("Invalid matrix start data");
                return;
            }
            // Calculate how did the combat actually end.
            var partyEndHp = new Dictionary<HeroProfession, float>();
            float totalLostMaxHp = 0;
            float totalHpLost = 0;
            foreach (var heroProfession in party.GetHeroProfessions())
            {
                totalLostMaxHp += 1 - party.GetMaxHpForHero(heroProfession) / initialMaxHp[heroProfession];
                totalHpLost += 1 - party.GetHpForHero(heroProfession) / initialHp[heroProfession];
                partyEndHp[heroProfession] = party.GetMaxHpForHero(heroProfession);
            }
            var partyStartHp = new Dictionary<HeroProfession, float>(initialMaxHp);
            var partyAttack = new Dictionary<HeroProfession, float>(partyDamageMultipliers);
            var finishedEncounter = encounter;
            // Send the combat results to the analytics backend.
            analyticsService?.LogCombat(partyStartHp, partyEndHp, partyAttack, finishedEncounter, expectedDifficulty.Value, totalLostMaxHp, wasGameOver, IsStaticEncounter, AdjustMatrixForNextFight);
            // And adjust the matrix if we should do that (we do not adjust the matrix during the static phase of the experiment).
            if (AdjustMatrixForNextFight)
            {
                AddMatrixRow(party, wasGameOver ? 3f : totalLostMaxHp, wasGameOver ? 3f : totalHpLost);
                // We start the adjustment on another thread so we have more time.
                AdjustMatrix(partyStartHp, partyAttack, finishedEncounter, expectedDifficulty.Value, totalLostMaxHp,
                    wasGameOver);
            }
            // Reset all the variables to prepare for the next encounter.
            AdjustMatrixForNextFight = true;
            initialMaxHp.Clear();
            partyDamageMultipliers.Clear();
            initialHp.Clear();
            expectedDifficulty = null;
            encounter = null;
        }
        /// <summary>
        /// This method is used to adjust the matrix after an encounter.
        /// Runs on another thread.
        /// </summary>
        /// <param name="partyStartHp">Start max HP of the party.</param>
        /// <param name="partyAttack">Attack of the party.</param>
        /// <param name="encounter">The enemies the party was fighting.</param>
        /// <param name="expectedDifficulty">The expected difficulty of the encounter.</param>
        /// <param name="realDifficulty">The actual difficulty measured after the combat was over.</param>
        /// <param name="wasGameOver">If true, the party was wiped.</param>
        private void AdjustMatrix(Dictionary<HeroProfession, float> partyStartHp, Dictionary<HeroProfession, float> partyAttack, EncounterDefinition encounter, float expectedDifficulty, float realDifficulty, bool wasGameOver)
        {
            // This starts the async operation on another thread.
            Task.Run(() =>
            {
                // Calculate how off we are and the monster power and party power of the enemies in the encounter.
                float resultsDifference = expectedDifficulty - realDifficulty;
                float partyPower = partyStartHp[HeroProfession.Ranger] * partyAttack[HeroProfession.Ranger] +
                                   partyStartHp[HeroProfession.Knight] * partyAttack[HeroProfession.Knight] +
                                   partyStartHp[HeroProfession.Cleric] * partyAttack[HeroProfession.Cleric];
                var encounterDifficulty = encounter.GetAdjustedMonsterCount(configuration);
                // Select the correct learning speed for this encounter.
                var learningSpeed = resultsDifference > 0
                    ? configuration.LearningSpeedIncreaseDifficulty
                    : configuration.LearningSpeedDecreaseDifficulty;
                // Calculate how much at most should we update the difficulty.
                var modifyDifficultyBy = resultsDifference * learningSpeed;
                // Go through all elements in the matrix and update them as necessary.
                foreach (var matrixElement in difficultyMatrix.MatrixElements)
                {
                    var largerPartyPower = Math.Max(matrixElement.PartyPower, partyPower);
                    var largerDifficulty =
                        Math.Max(matrixElement.EncounterGroups.GetAdjustedMonsterCount(configuration),
                            encounterDifficulty);
                    // We divide the difference it by party power and encounter difficulty to more accurately gouge just how much difference there is.
                    // Difference between encounter with difficulty 46000 and 42000 is quite minor, but difference between 6000 and 2000 is huge.
                    // Same goes for party power.
                    // This should work no matter the values used for party power and enemy power.
                    var partyPowerDifference = Math.Abs(partyPower - matrixElement.PartyPower) / largerPartyPower;
                    var encounterDifference = encounter.GetDistance(matrixElement.EncounterGroups, configuration) /
                                              largerDifficulty;
                    // From the differences calculate the similarity between the current element and the encounter that just ended.
                    var totalDifference = partyPowerDifference + encounterDifference;
                    var similarity = 1 - totalDifference;
                    similarity = similarity < configuration.LearningMinimumSimilarity
                        ? configuration.LearningMinimumSimilarity
                        : similarity;
                    // Finally update the element in the matrix appropriately.
                    if (configuration.EmulateV1Bug)
                    {
                        // Wrong behavior, fixed, but we need it to be here for analyzing data from the original version.
                        matrixElement.ResourcesLost -= matrixElement.ResourcesLost * modifyDifficultyBy * similarity; 
                    } 
                    else
                    {
                        matrixElement.ResourcesLost -= modifyDifficultyBy * similarity;
                    }
                    // Make sure to stay in bounds.
                    if (matrixElement.ResourcesLost > 3)
                    {
                        matrixElement.ResourcesLost = 3;
                    }
                    else if (matrixElement.ResourcesLost < 0)
                    {
                        matrixElement.ResourcesLost = 0;
                    }
                }
                // Raise the event which will log the new matrix.
                MatrixChanged?.Invoke(this, new MatrixChangedEventArgs
                {
                    DifficultyEstimate = expectedDifficulty,
                    DifficultyReality = realDifficulty,
                    FoughtEncounter = encounter,
                    PartyAttack = partyAttack,
                    PartyHitpoints = partyStartHp,
                    WasGameOver = wasGameOver,
                });
            });
        }
        /// <summary>
        /// Add the row representing the combat that just ended to the matrix.
        /// </summary>
        /// <param name="party">The party involved in combat.</param>
        /// <param name="maxHpLost">Sum of percentages of lost max HP</param>
        /// <param name="hpLost">Sum of percentages of lost HP</param>
        private void AddMatrixRow(IPartyDefinition party, float maxHpLost, float hpLost)
        {
            var heroStatusMap = new Dictionary<HeroProfession, HeroCombatStatus>();
            float partyStrength = 0;
            foreach (var heroProfession in party.GetHeroProfessions())
            {
                partyStrength += initialMaxHp[heroProfession] * partyDamageMultipliers[heroProfession];
                heroStatusMap[heroProfession] = new HeroCombatStatus
                {
                    Attack = partyDamageMultipliers[heroProfession],
                    Hp = initialHp[heroProfession],
                    HpLost = 1 - party.GetHpForHero(heroProfession) / initialHp[heroProfession],
                    MaxHp = initialMaxHp[heroProfession],
                    MaxHpLost = 1 - party.GetMaxHpForHero(heroProfession) / initialMaxHp[heroProfession],
                    WasKilled = party.IsDown(heroProfession)
                };
            }
            var matrixLine = new DifficultyMatrixSourceLine
            {
                TestIndex = -1,
                EncounterDefinition = encounter,
                HeroCombatStatuses = heroStatusMap,
                HpLost = hpLost,
                MaxHpLost = maxHpLost,
                MonsterTier = -1,
                PartyConfiguration = "Real combat",
                PartyStrength = partyStrength
            };
            difficultyMatrix.MatrixElements.Add(new EncounterDifficultyMatrixElement(matrixLine));
        }
    }
    /// <summary>
    /// The event args for the event that is raised the matrix is changed.
    /// </summary>
    public class MatrixChangedEventArgs
    {
        /// <summary>
        /// The start hit points of the characters.
        /// </summary>
        public Dictionary<HeroProfession, float> PartyHitpoints;
        /// <summary>
        /// The start attack of the characters.
        /// </summary>
        public Dictionary<HeroProfession, float> PartyAttack;
        /// <summary>
        /// Enemies in the encounter that just ended.
        /// </summary>
        public EncounterDefinition FoughtEncounter;
        /// <summary>
        /// Difficulty as estimated by the matrix.
        /// </summary>
        public float DifficultyEstimate;
        /// <summary>
        /// The actual difficulty of the encounter.
        /// </summary>
        public float DifficultyReality;
        /// <summary>
        /// If true, the party was wiped in the encounter.
        /// </summary>
        public bool WasGameOver;
    }
}
