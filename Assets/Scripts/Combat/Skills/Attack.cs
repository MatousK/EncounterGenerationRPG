using System;

namespace Assets.Scripts.Combat.Skills
{
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

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Update()
        {
            base.Update();
        }

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
