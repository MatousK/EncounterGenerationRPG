using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class AutoAttacking : MonoBehaviour
{
    [NonSerialized]
    public CombatantBase Target;
    public TargetedSkill AutoAttackSkill;
    CombatantBase SelfCombatant;

    private void Awake()
    {
        SelfCombatant = GetComponent<CombatantBase>();
    }

    private void Update()
    {
        if (Target && !Target.IsTargetable)
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
        if (SelfCombatant.IsBlockingSkillInProgress(false))
        {
            return;
        }
        // We are not doing anything interesting - just attack the target.
        AutoAttackSkill.UseSkillOn(Target);
    }
}