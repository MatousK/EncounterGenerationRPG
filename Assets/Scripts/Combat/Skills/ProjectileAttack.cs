using System;
using Assets.Scripts.Effects.Projectiles;
using UnityEngine;

namespace Assets.Scripts.Combat.Skills
{
    /// <summary>
    /// An attack skill that launches a projectile. Once the animation finishes, a projectile is spawned and sent flying towards the target.
    /// Only when it hits the target is the skill effect applied.
    /// </summary>
    public class ProjectileAttack : Attack
    {
        /// <summary>
        /// Projectile that should be spawned when the attack is started.
        /// Should be subobject of the character/monster and should be in the appropriate position relative to the monster.
        /// </summary>
        public GameObject ProjectileTemplate;
        /// <summary>
        /// How many squares should the projectile traverse per second.
        /// </summary>
        public float ProjectileSpeed;
        protected override void Start()
        {
            DealDamageOnApplySkillEffects = false;
            base.Start();
        }

        protected override void Update()
        {
            base.Update();
        }
        /// <summary>
        /// Applying this skill effect means launching the projectile this skill represents.
        /// </summary>
        /// <param name="sender">The sender of this event.</param>
        /// <param name="e">Ignored.</param>
        protected override void ApplySkillEffects(object sender, EventArgs e)
        {
            var newProjectile = Instantiate(ProjectileTemplate, transform, true);
            newProjectile.transform.parent = null;
            newProjectile.SetActive(true);
            // Target could change before the projectile hits, so put it in a variable just to be sure.
            var originalTarget = Target;
            newProjectile.GetComponent<Projectile>().StartProjectile(Target, ProjectileSpeed, () => OnProjectileHit(originalTarget));
            base.ApplySkillEffects(sender, e);
        }
        /// <summary>
        /// Called when the projectile hits its target.
        /// </summary>
        /// <param name="originalTarget">The target hit by the projectile.</param>
        protected virtual void OnProjectileHit(CombatantBase originalTarget)
        {
            originalTarget.TakeDamage(DamagePerHit, SelfCombatant);
            CombatSoundsController.ProjectileHit(SkillSoundType);
        }
    }
}