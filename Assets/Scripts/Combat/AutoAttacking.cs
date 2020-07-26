using System;
using Assets.Scripts.Combat.Skills;
using UnityEngine;

namespace Assets.Scripts.Combat
{
    /// <summary>
    /// A script which will repeatedly use some skill on at target if a target is set, targetable and alive.
    /// </summary>
    class AutoAttacking : MonoBehaviour
    {
        /// <summary>
        /// The target of the auto attack,
        /// </summary>
        [NonSerialized]
        public CombatantBase Target;
        /// <summary>
        /// The skill used for automatic attacks, usually a basic attack.
        /// </summary>
        public TargetedSkill AutoAttackSkill = null;
        /// <summary>
        /// The combatant who has this auto attacking script.
        /// </summary>
        private CombatantBase selfCombatant;

        private void Start()
        {
            selfCombatant = GetComponent<CombatantBase>();
        }
        /// <summary>
        /// As long as the target is alive and targetable and we are not using any skills, use basic attack on the target.
        /// </summary>
        private void Update()
        {
            if (Target && !Target.CanBeTargeted || selfCombatant.IsDown)
            {
                // Target is dead or invincible, no sense in beating a dead horse or a god.
                // Or we are down, so, again, not much to do, really.
                Target = null;
            }
            if (Target == null)
            {
                AutoAttackSkill.TryStopSkill();
                // Noone to autoattack.
                return;
            }
            // Do not start an autoattack if we're already doing blocking skills, as basic attack is also a skill.
            if (selfCombatant.IsBlockingSkillInProgress(false))
            {
                return;
            }
            // We are not doing anything interesting - just attack the target.
            AutoAttackSkill.UseSkillOn(Target);
        }
    }
}