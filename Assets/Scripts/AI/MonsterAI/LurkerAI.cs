using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class LurkerAI: MonsterAIBase
{
    public float TeleportSkillMinDistance = 3;
    public TargetedSkill TeleportSkill;

    protected override void Update()
    {
        base.Update();
    }

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnActionRequired()
    {
        // Teleport to the current target if too far away, or just start attacking the strongest enemy as much as possible.
        var target = GetCurrentTarget();
        var distanceToTarget = Vector2.Distance(target.transform.position, ControlledCombatant.transform.position);
        if (distanceToTarget > TeleportSkillMinDistance && TryUseSkill(target, TeleportSkill))
        {
            return;
        }
        base.OnActionRequired();
    }

    protected override CombatantBase GetCurrentTarget()
    {
        return ForcedTarget != null ? ForcedTarget : GetStrongestHero();
    }
}