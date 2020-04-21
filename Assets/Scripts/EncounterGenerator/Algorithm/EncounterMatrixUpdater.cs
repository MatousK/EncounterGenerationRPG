using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Combat;
using Assets.Scripts.EncounterGenerator.Model;
using UnityEngine;

namespace Assets.Scripts.EncounterGenerator.Algorithm
{
    /// <summary>
    /// After a fight is over, this class should update the difficulty matrix reflecting the combat result.
    /// </summary>
    public class EncounterMatrixUpdater
    {
        public EncounterMatrixUpdater(EncounterDifficultyMatrix difficultyMatrix)
        {
            this.difficultyMatrix = difficultyMatrix;
        }

        private EncounterDifficultyMatrix difficultyMatrix;
        private readonly Dictionary<HeroProfession, float> initialMaxHp = new Dictionary<HeroProfession, float>();
        private readonly Dictionary<HeroProfession, float> initialHp = new Dictionary<HeroProfession, float>();
        private readonly Dictionary<HeroProfession, float> partyDamageMultipliers = new Dictionary<HeroProfession, float>();
        private float? expectedDifficulty;
        private EncounterDefinition encounter;

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
                return;;
            }
            float totalLostMaxHp = 0;
            float totalHpLost = 0;
            foreach (var hero in party.PartyMembers)
            {
                totalLostMaxHp += 1 - hero.MaxHitpoints / initialMaxHp[hero.HeroProfession];
                totalHpLost += 1 - hero.HitPoints / initialHp[hero.HeroProfession];
            }
            AddMatrixRow(party, wasGameOver ? 3f : totalLostMaxHp, wasGameOver ? 3f : totalHpLost);
            float resultsDifference = expectedDifficulty.Value - totalLostMaxHp;
            // TODO: Adjust matrix.
            initialMaxHp.Clear();
            expectedDifficulty = null;
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
}
