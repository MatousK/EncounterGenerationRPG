using System;

namespace Assets.Scripts.Combat
{
    /// <summary>
    /// List of attributes a combatant can have that can change his stats.
    /// </summary>
    [Serializable]
    public class CombatantAttributes
    {
        /// <summary>
        /// All damage the combatant will deal will be multiplied by this.
        /// </summary>
        public float DealtDamageMultiplier = 1;
        /// <summary>
        /// All damage the combatant will receive will be multiplied by this.
        /// </summary>
        public float ReceivedDamageMultiplier = 1;
        /// <summary>
        /// Multiplier for the speed of attack animations.
        /// </summary>
        public float AttackSpeedMultiplier = 1;
        /// <summary>
        /// Modifies the movement speed of the combatant.
        /// </summary>
        public float MovementSpeedMultiplier = 1;
        /// <summary>
        /// Increases the range of all skills of the combatant,
        /// </summary>
        public float RangeMultiplier = 1;
        /// <summary>
        /// All healing done by this combatant will be multiplied by this number.
        /// </summary>
        public float DealtHealingMultiplier = 1;
        /// <summary>
        /// All healing received by this combatant will be multiplied by this number.
        /// </summary>
        public float ReceivedHealingMultiplier = 1;
        /// <summary>
        /// Hit points per second that will be healed while not in combat.
        /// </summary>
        public float OutOfCombatHealthRegeneration = 20;
        /// <summary>
        /// Hit points per second that will be healed in combat.
        /// </summary>
        public float CombatHealthRegeneration = 0;
    }
}
