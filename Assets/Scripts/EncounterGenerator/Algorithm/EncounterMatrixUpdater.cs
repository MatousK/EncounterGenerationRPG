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

        public void StoreCombatStartConditions(PartyDefinition party, EncounterDefinition encounter, float expectedDifficulty)
        {
            foreach (var hero in party.PartyMembers)
            {
                initialMaxHp[hero.HeroProfession] = hero.MaxHitpoints;
                initialHp[hero.HeroProfession] = hero.HitPoints;
                partyDamageMultipliers[hero.HeroProfession] = hero.Attributes.DealtDamageMultiplier;
            }
            this.expectedDifficulty = expectedDifficulty;
            this.encounter = encounter;
        }

        public void CombatOverAdjustMatrix(PartyDefinition party, bool wasGameOver)
        {
            if (expectedDifficulty == null || !initialMaxHp.Any())
            {
                UnityEngine.Debug.LogWarning("Logging combat encounter when initial conditions are not set");
                return; ;
            }

            var partyEndHp = new Dictionary<HeroProfession, float>();
            float totalLostMaxHp = 0;
            float totalHpLost = 0;
            foreach (var hero in party.PartyMembers)
            {
                totalLostMaxHp += 1 - hero.MaxHitpoints / initialMaxHp[hero.HeroProfession];
                totalHpLost += 1 - hero.HitPoints / initialHp[hero.HeroProfession];
                partyEndHp[hero.HeroProfession] = hero.MaxHitpoints;
            }
            var partyStartHp = new Dictionary<HeroProfession, float>(initialMaxHp);
            var partyAttack = new Dictionary<HeroProfession, float>(partyDamageMultipliers);
            var finishedEncounter = encounter;
            analyticsService.LogCombat(partyStartHp, partyEndHp, partyAttack, finishedEncounter, expectedDifficulty.Value, totalLostMaxHp, wasGameOver, IsStaticEncounter, AdjustMatrixForNextFight);
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
                    matrixElement.ResourcesLost -= matrixElement.ResourcesLost * modifyDifficultyBy * similarity;
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

        private void AddMatrixRow(PartyDefinition party, float maxHpLost, float hpLost)
        {
            var heroStatusMap = new Dictionary<HeroProfession, HeroCombatStatus>();
            float partyStrength = 0;
            foreach (var hero in party.PartyMembers)
            {
                partyStrength += initialMaxHp[hero.HeroProfession] * partyDamageMultipliers[hero.HeroProfession];
                heroStatusMap[hero.HeroProfession] = new HeroCombatStatus
                {
                    Attack = partyDamageMultipliers[hero.HeroProfession],
                    Hp = initialHp[hero.HeroProfession],
                    HpLost = 1 - hero.HitPoints / initialHp[hero.HeroProfession],
                    MaxHp = initialMaxHp[hero.HeroProfession],
                    MaxHpLost = 1 - hero.MaxHitpoints / initialMaxHp[hero.HeroProfession],
                    WasKilled = hero.IsDown
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
