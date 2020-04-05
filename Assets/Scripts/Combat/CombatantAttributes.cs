using System;

namespace Assets.Scripts.Combat
{
    [Serializable]
    public class CombatantAttributes
    {
        public float DealtDamageMultiplier = 1;
        public float ReceivedDamageMultiplier = 1;
        public float AttackSpeedMultiplier = 1;
        public float MovementSpeedMultiplier = 1;
        public float RangeMultiplier = 1;
        public float DealtHealingMultiplier = 1;
        public float ReceivedHealingMultiplier = 1;
        public float OutOfCombatHealthRegeneration = 20;
        public float CombatHealthRegeneration = 0;
    }
}
