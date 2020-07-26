using System;

namespace Assets.Scripts.Combat.Skills.Monster.Leader
{
    /// <summary>
    /// A healing skill that heals the target for some amount.
    /// </summary>
    public class HealLightWounds : TargetedGestureSkill
    {
        /// <summary>
        /// The amount for which this heal should heal.
        /// </summary>
        public int HealAmount;
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void Start()
        {
            base.Start();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void Update()
        {
            base.Update();
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="sender"><inheritdoc/></param>
        /// <param name="e"><inheritdoc/></param>
        protected override void ApplySkillEffects(object sender, EventArgs e)
        {
            Target.HealDamage(HealAmount, SelfCombatant);
            base.ApplySkillEffects(sender, e);
        }
    }
}
