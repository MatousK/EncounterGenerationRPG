using System;
using System.Threading;
using Assets.Scripts.Combat;

namespace Assets.Scripts.CombatSimulator.PartyConfigurationProvider
{
    /// <summary>
    /// Can retrieve a party configuration based on how many powerups did the party pickup.
    /// </summary>
    public abstract class PartyConfigurationProvider
    {
        public float SimulateDamagedPartyProbability = 0.2f;
        // These should really be a constant somewhere
        protected const int ClericBaseHp = 125;
        protected const int RangerBaseHp = 50;
        protected const int KnightBaseHp = 250;

        protected const float RangerBaseAttack = 30;
        protected const float KnightBaseAttack = 15;
        protected const float ClericBaseAttack = 10;

        protected const float RangerAttackIncrement = 12;
        protected const float KnightAttackIncrement = 8;
        protected const float ClericAttackIncrement = 4;

        protected const float KnightHpIncrement = 60;
        protected const float ClericHpIncrement = 40;
        protected const float RangerHpIncrement = 20;

        public abstract PartyConfiguration GetPartyConfiguration();

        private float GetAttributeWithUpgrades(float baseValue, int incrementCount, float increment,
            float healthPercentage)
        {
            return (baseValue + (increment * incrementCount)) * healthPercentage;
        }

        protected PartyMemberConfiguration GetStats(HeroProfession profession, int healthPowerups, int attackPowerups)
        {
            bool shouldSimulateDamagedParty = UnityEngine.Random.Range(0f, 1f) < SimulateDamagedPartyProbability;
            float rangerHpPercentage = shouldSimulateDamagedParty ? UnityEngine.Random.Range(0.05f, 1f) : 1f;
            float knightHpPercentage = shouldSimulateDamagedParty ? UnityEngine.Random.Range(0.05f, 1f) : 1f;
            float clericHpPercentage = shouldSimulateDamagedParty ? UnityEngine.Random.Range(0.05f, 1f) : 1f;
            float baseHp;
            float baseAttack;
            float attackIncrement;
            float hpIncrement;
            float hpPercentage;
            switch (profession)
            {
                case HeroProfession.Cleric:
                    baseHp = ClericBaseHp;
                    baseAttack = ClericBaseAttack;
                    attackIncrement = ClericAttackIncrement;
                    hpIncrement = ClericHpIncrement;
                    hpPercentage = clericHpPercentage;
                    break;
                case HeroProfession.Knight:
                    baseHp = KnightBaseHp;
                    baseAttack = KnightBaseAttack;
                    attackIncrement = KnightAttackIncrement;
                    hpIncrement = KnightHpIncrement;
                    hpPercentage = knightHpPercentage;
                    break;
                case HeroProfession.Ranger:
                    baseHp = RangerBaseHp;
                    baseAttack = RangerBaseAttack;
                    attackIncrement = RangerAttackIncrement;
                    hpIncrement = RangerHpIncrement;
                    hpPercentage = rangerHpPercentage;
                    break;
                default:
                    throw new ArgumentException("Profession is not supported");

            }
            return new PartyMemberConfiguration
            {
                AttackModifier = GetAttributeWithUpgrades(baseAttack, attackPowerups, attackIncrement, 1),
                MaxHp = (int)GetAttributeWithUpgrades(baseHp, healthPowerups, hpIncrement, hpPercentage)
            };
        }
    }
}