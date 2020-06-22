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
        /// Flags controls whether current combat should update the matrix or not.
        /// </summary>
        public bool AdjustMatrixForNextFight = true;
        /// <summary>
        /// If true, currently handled fight is a static encounter.
        /// </summary>
        public bool IsStaticEncounter;
        private readonly EncounterGeneratorConfiguration configuration;
        private readonly EncounterDifficultyMatrix difficultyMatrix;
        private readonly Dictionary<HeroProfession, float> initialMaxHp = new Dictionary<HeroProfession, float>();
        private readonly Dictionary<HeroProfession, float> initialHp = new Dictionary<HeroProfession, float>();
        private readonly Dictionary<HeroProfession, float> partyDamageMultipliers = new Dictionary<HeroProfession, float>();
        private float? expectedDifficulty;
        private EncounterDefinition encounter;
        private AnalyticsService analyticsService;

        public void StoreCombatStartConditions(IPartyDefinition party, EncounterDefinition encounter, float expectedDifficulty)
        {
            foreach (var heroProfession in party.GetHeroProfessions())
            {
                initialMaxHp[heroProfession] = party.GetMaxHpForHero(heroProfession);
                initialHp[heroProfession] = party.GetHpForHero(heroProfession);
                partyDamageMultipliers[heroProfession] = party.GetAttackForHero(heroProfession);
            }
            if (initialMaxHp.Any(p => p.Value == 0) && !configuration.EmulateV1Bug)
            {
                UnityEngine.Debug.LogError("Invalid matrix start data");
                return;
            }
            this.expectedDifficulty = expectedDifficulty;
            this.encounter = encounter;
        }

        public void CombatOverAdjustMatrix(IPartyDefinition party, bool wasGameOver)
        {
            if (expectedDifficulty == null || !initialMaxHp.Any())
            {
                // Probably combat simulator, in release mode this would be a bug.
                UnityEngine.Debug.LogWarning("Logging combat encounter when initial conditions are not set");
                return; ;
            }
            if (initialMaxHp.Any(p => p.Value == 0) && !configuration.EmulateV1Bug)
            {
                UnityEngine.Debug.LogError("Invalid matrix start data");
                return;
            }

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
            analyticsService?.LogCombat(partyStartHp, partyEndHp, partyAttack, finishedEncounter, expectedDifficulty.Value, totalLostMaxHp, wasGameOver, IsStaticEncounter, AdjustMatrixForNextFight);
            if (AdjustMatrixForNextFight)
            {
                AddMatrixRow(party, wasGameOver ? 3f : totalLostMaxHp, wasGameOver ? 3f : totalHpLost);
                // We start the adjustment on another thread so we have more time.
                AdjustMatrix(partyStartHp, partyAttack, finishedEncounter, expectedDifficulty.Value, totalLostMaxHp,
                    wasGameOver);
            }

            AdjustMatrixForNextFight = true;
            initialMaxHp.Clear();
            partyDamageMultipliers.Clear();
            initialHp.Clear();
            expectedDifficulty = null;
            encounter = null;
        }

        private void AdjustMatrix(Dictionary<HeroProfession, float> partyStartHp, Dictionary<HeroProfession, float> partyAttack, EncounterDefinition encounter, float expectedDifficulty, float realDifficulty, bool wasGameOver)
        {
            Task.Run(() =>
            {
                float resultsDifference = expectedDifficulty - realDifficulty;
                // TODO: Think of a better algorithm. 
                float partyPower = partyStartHp[HeroProfession.Ranger] * partyAttack[HeroProfession.Ranger] +
                                   partyStartHp[HeroProfession.Knight] * partyAttack[HeroProfession.Knight] +
                                   partyStartHp[HeroProfession.Cleric] * partyAttack[HeroProfession.Cleric];
                var encounterDifficulty = encounter.GetAdjustedMonsterCount(configuration);
                var learningSpeed = resultsDifference > 0
                    ? configuration.LearningSpeedIncreaseDifficulty
                    : configuration.LearningSpeedDecreaseDifficulty;
                var modifyDifficultyBy = resultsDifference * learningSpeed;
                foreach (var matrixElement in difficultyMatrix.MatrixElements)
                {
                    var largerPartyPower = Math.Max(matrixElement.PartyPower, partyPower);
                    var largerDifficulty =
                        Math.Max(matrixElement.EncounterGroups.GetAdjustedMonsterCount(configuration),
                            encounterDifficulty);
                    // We divide the difference it by party power and encounter difficulty to more accurately gouge just how much difference there is.
                    // Difference between encounter with difficulty 46000 and 42000 is quite minor, but difference between 6000 and 2000 is huge.
                    // Same goes for party power.
                    // This way
                    var partyPowerDifference = Math.Abs(partyPower - matrixElement.PartyPower) / largerPartyPower;
                    var encounterDifference = encounter.GetDistance(matrixElement.EncounterGroups, configuration) /
                                              largerDifficulty;
                    var totalDifference = partyPowerDifference + encounterDifference;
                    var similarity = 1 - totalDifference;
                    similarity = similarity < configuration.LearningMinimumSimilarity
                        ? configuration.LearningMinimumSimilarity
                        : similarity;
                    if (configuration.EmulateV1Bug)
                    {
                        // Wrong behavior, fixed, but we need it to be here for analyzing data from the original version.
                        matrixElement.ResourcesLost -= matrixElement.ResourcesLost * modifyDifficultyBy * similarity; 
                    } 
                    else
                    {
                        matrixElement.ResourcesLost -= modifyDifficultyBy * similarity;
                    }
                    if (matrixElement.ResourcesLost > 3)
                    {
                        matrixElement.ResourcesLost = 3;
                    }
                    else if (matrixElement.ResourcesLost < 0)
                    {
                        matrixElement.ResourcesLost = 0;
                    }
                }
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

    public class MatrixChangedEventArgs
    {
        public Dictionary<HeroProfession, float> PartyHitpoints;
        public Dictionary<HeroProfession, float> PartyAttack;
        public EncounterDefinition FoughtEncounter;
        public float DifficultyEstimate;
        public float DifficultyReality;
        public bool WasGameOver;
    }
}
