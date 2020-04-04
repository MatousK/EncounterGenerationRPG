using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class LeaderAI: MonsterAIBase
{
    public float HealingAuraMoveToRange = 2;
    public TargetedSkill TargetHeroSkill;

    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Update()
    {
        base.Update();
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    protected override void OnActionRequired()
    {
        var target = GetCurrentTarget() as Hero;
        var alliedMonsters = CombatantsManager.GetOpponentsFor(ControlledCombatant, onlyAlive: true).ToArray();
        // Targeting is unnecessary when fighting alongside a low amount of monsters.
        if (target.AITargetPriority == AITargetPriority.High && alliedMonsters.Length >= 3)
        {
            // High priority target is alive, target him and kill him quickly.
            if (TryUseSkill(target, TargetHeroSkill))
            {
                return;
            }
        }
        if (AdvancedSkill.IsBeingUsed())
        {
            // The advanced skill is for healing. Stay close to the most wounded ally.
            CombatantBase mostWoundedAlly = null;
            float mostWoundedAllyHPPercentage = 1;
            foreach (var ally in alliedMonsters)
            {
                var allyHPPercentage = ally.HitPoints / ally.MaxHitpoints;
                if (allyHPPercentage < mostWoundedAllyHPPercentage && ally != ControlledCombatant)
                {
                    mostWoundedAllyHPPercentage = allyHPPercentage;
                    mostWoundedAlly = ally;
                }
            }
            if (mostWoundedAlly != null)
            {
                if (Vector2.Distance(mostWoundedAlly.transform.position, ControlledCombatant.transform.position) > HealingAuraMoveToRange)
                {
                    ControlledCombatant.GetComponent<MovementController>().MoveToPosition(mostWoundedAlly.transform.position);
                    return;
                }
            }
        }
        base.OnActionRequired();
    }

    protected override CombatantBase GetCurrentTarget()
    {
        return ForcedTarget != null ? ForcedTarget : GetStrongestHero();
    }

    protected override CombatantBase GetAdvancedSkillTarget()
    {
        return ControlledCombatant;
    }
}
