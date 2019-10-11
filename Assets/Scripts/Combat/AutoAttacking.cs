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

    private void Start()
    {
        SelfCombatant = GetComponent<CombatantBase>();
    }

    private void Update()
    {
        if (Target && Target.IsDown)
        {
            // Target is dead, no sense in beating a dead horse.
            Target = null;
        }
        if (Target == null)
        {
            // Noone to autoattack.
            return;
        }
        // Do not start an autoattack if we're already doing blocking skills, as basic attack is also a skill.
        if (SelfCombatant.IsSkillUsageBlocked())
        {
            return;
        }
        // We are not doing anything interesting - just attack the target.
        AutoAttackSkill.UseSkillOn(Target);
    }
}