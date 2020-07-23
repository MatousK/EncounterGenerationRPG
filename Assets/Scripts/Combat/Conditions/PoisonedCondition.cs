using UnityEngine;

namespace Assets.Scripts.Combat.Conditions
{
    /// <summary>
    /// Condition that specifies that this combatant is poisoned. This reduces his attack and movement speed and shows a poisoned effect.
    /// </summary>
    public class PoisonedCondition : ConditionBase
    {
        /// <summary>
        /// The modifier of this condition.
        /// </summary>
        public float PoisonSpeedModifier = 0.5f;
        /// <summary>
        /// The combatant who has this condtion.
        /// </summary>
        private CombatantBase selfCombatant;
        /// <summary>
        /// Apply the effects of the condition, i.e. start the animation and reduce attack and movement speed.
        /// </summary>
        protected override void StartCondition()
        {
            base.StartCondition();
            selfCombatant = GetComponentInParent<CombatantBase>();
            selfCombatant.Attributes.AttackSpeedMultiplier *= PoisonSpeedModifier;
            selfCombatant.Attributes.MovementSpeedMultiplier *= PoisonSpeedModifier;
            selfCombatant.GetComponent<Animator>().SetBool("Poisoned", true);
        }
        /// <summary>
        /// Remove the effects of the condition, i.e. end the animation and restore attack and movement speed.
        /// </summary>
        protected override void EndCondition()
        {
            base.EndCondition();
            selfCombatant.Attributes.AttackSpeedMultiplier /= PoisonSpeedModifier;
            selfCombatant.Attributes.MovementSpeedMultiplier /= PoisonSpeedModifier;
            selfCombatant.GetComponent<Animator>().SetBool("Poisoned", false);
        }
    }
}