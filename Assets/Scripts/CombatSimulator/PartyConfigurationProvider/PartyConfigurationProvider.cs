using System;
using System.Threading;
using Assets.Scripts.Combat;

namespace Assets.Scripts.CombatSimulator.PartyConfigurationProvider
{
    /// <summary>
    /// Used by the combat simulator, this class can create a party configuration which a player would have if a player followed some specific strategy of distributing pick ups.
    /// </summary>
    public abstract class PartyConfigurationProvider
    {
        /// <summary>
        /// A chance that this party will be simulated in damaged state were the members were hurt in combat before.
        /// Alternative is the party being alright.
        /// </summary>
        public float SimulateDamagedPartyProbability = 0.5f;
        // TODO: These should be gotten from somewhere else.
        /// <summary>
        /// Cleric's starting hit points.
        /// </summary>
        protected const int ClericBaseHp = 125;
        /// <summary>
        /// Ranger's starting hit points.
        /// </summary>
        protected const int RangerBaseHp = 50;
        /// <summary>
        /// Knight's starting hit points.
        /// </summary>
        protected const int KnightBaseHp = 250;

        /// <summary>
        /// Ranger's starting attack.
        /// </summary>
        protected const float RangerBaseAttack = 30;
        /// <summary>
        /// Knight's starting attack.
        /// </summary>
        protected const float KnightBaseAttack = 15;
        /// <summary>
        /// Cleric's starting attack.
        /// </summary>
        protected const float ClericBaseAttack = 10;

        /// <summary>
        /// How much will ranger's attack increase per damage upgrade.
        /// </summary>
        protected const float RangerAttackIncrement = 12;
        /// <summary>
        /// How much will knight's attack increase per damage upgrade.
        /// </summary>
        protected const float KnightAttackIncrement = 8;
        /// <summary>
        /// How much will cleric's attack increase per damage upgrade.
        /// </summary>
        protected const float ClericAttackIncrement = 4;

        /// <summary>
        /// How much will knight's max HP increase per health upgrade.
        /// </summary>
        protected const float KnightHpIncrement = 60;
        /// <summary>
        /// How much will cleric's max HP increase per health upgrade.
        /// </summary>
        protected const float ClericHpIncrement = 40;
        /// <summary>
        /// How much will ranger's max HP increase per health upgrade.
        /// </summary>
        protected const float RangerHpIncrement = 20;

        /// <summary>
        /// Creates a party configuration represented by this strategy.
        /// </summary>
        /// <returns>The party configuration this object represents.</returns>
        public abstract PartyConfiguration GetPartyConfiguration();

        /// <summary>
        /// Will calculate the value of some attribute after a specified number of upgrades were picked up.
        /// </summary>
        /// <param name="baseValue">The starting value of this attribute.</param>
        /// <param name="incrementCount">How many times was an upgrade picked up.</param>
        /// <param name="increment">Increase of the attribute per upgrade.</param>
        /// <param name="healthPercentage">The results will be multiplied by this. It simulates a damaged party.</param>
        /// <returns>The value of the attribute under these circumstances.</returns>
        private float GetAttributeWithUpgrades(float baseValue, int incrementCount, float increment,
            float healthPercentage)
        {
            return (baseValue + (increment * incrementCount)) * healthPercentage;
        }
        /// <summary>
        /// Retrieves the attributes of a specified character after he picks up the specified powerups. Might simulate damaged party members randomly, <see cref="SimulateDamagedPartyProbability"></see>
        /// </summary>
        /// <param name="profession">Class of the character whose attributes are requested.</param>
        /// <param name="healthPowerups">How many health power ups did this character pick up.</param>
        /// <param name="attackPowerups">How many damage power ups did this character pick up.</param>
        /// <returns>The configuration of the hero.</returns>
        protected PartyMemberConfiguration GetStats(HeroProfession profession, int healthPowerups, int attackPowerups)
        {
            bool shouldSimulateDamagedParty = UnityEngine.Random.Range(0f, 1f) < SimulateDamagedPartyProbability;
            float rangerHpPercentage = shouldSimulateDamagedParty ? UnityEngine.Random.Range(0.2f, 1f) : 1f;
            float knightHpPercentage = shouldSimulateDamagedParty ? UnityEngine.Random.Range(0.2f, 1f) : 1f;
            float clericHpPercentage = shouldSimulateDamagedParty ? UnityEngine.Random.Range(0.2f, 1f) : 1f;
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