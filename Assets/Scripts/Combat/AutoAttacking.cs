using System;
using Assets.Scripts.Combat.Skills;
using UnityEngine;

namespace Assets.Scripts.Combat
{
    class AutoAttacking : MonoBehaviour
    {
        [NonSerialized]
        public CombatantBase Target;
        public TargetedSkill AutoAttackSkill = null;
        private CombatantBase selfCombatant;

        private void Awake()
        {
            selfCombatant = GetComponent<CombatantBase>();
        }

        private void Update()
        {
            if (Target && !Target.CanBeTargeted)
            {
                // Target is dead or invincible, no sense in beating a dead horse or a god.
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