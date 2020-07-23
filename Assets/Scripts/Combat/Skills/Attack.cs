using System;

namespace Assets.Scripts.Combat.Skills
{
    /// <summary>
    /// A targeted skill that is an attack, i.e. it deals damage to the target.
    /// </summary>
    public abstract class Attack : TargetedSkill
    {
        /// <summary>
        /// How much damage does the attack do per hit.
        /// </summary>
        public int DamagePerHit = 1;
        /// <summary>
        /// If true, damage will be automatically dealt when ApplySkillEffects is called.
        /// </summary>
        protected bool DealDamageOnApplySkillEffects = true;

        protected Attack()
        {
            SkillAnimationName = "Attacking";
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {
            base.Update();
        }
        /// <summary>
        /// Called when the animation hits the point where the attack should deal damage. And deals that damage.
        /// </summary>
        /// <param name="sender">Sender of this event.</param>
        /// <param name="e">Ignored.</param>
        protected override void ApplySkillEffects(object sender, EventArgs e)
        {
            if (Target != null && DealDamageOnApplySkillEffects)
            {
                Target.TakeDamage(DamagePerHit, SelfCombatant);
            }
            base.ApplySkillEffects(sender, e);
        }
    }
}
