using System;

namespace Assets.Scripts.CombatSimulator.PartyConfigurationProvider
{
    /// <summary>
    /// Can retrieve a party configuration based on how many powerups did the party pickup.
    /// </summary>
    public abstract class PartyConfigurationProvider
    {
        protected const int ClericBaseHp = 125;
        protected const int RangerBaseHp = 50;
        protected const int KnightBaseHp = 250;

        protected const float RangerBaseAttack = 30;
        protected const float KnightBaseAttack = 15;
        protected const float ClericBaseAttack = 10;

        public abstract PartyConfiguration GetPartyConfiguration();

        protected PartyMemberConfiguration GetStats(int baseHp, int healthPowerups, float baseAttack, int attackPowerups)
        {
            return new PartyMemberConfiguration { AttackModifier = (float)(baseAttack * Math.Pow(1.2, attackPowerups)), MaxHp = (int)(baseHp * Math.Pow(1.2, healthPowerups)) };
        }
    }
}