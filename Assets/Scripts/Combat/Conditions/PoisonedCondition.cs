using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PoisonedCondition : ConditionBase
{
    public float PoisonSpeedModifier = 0.5f;
    private CombatantBase selfCombatant;
    protected override void StartCondition()
    {
        base.StartCondition();
        selfCombatant = GetComponentInParent<CombatantBase>();
        selfCombatant.Attributes.AttackSpeedMultiplier *= PoisonSpeedModifier;
        selfCombatant.Attributes.MovementSpeedMultiplier *= PoisonSpeedModifier;
        selfCombatant.GetComponent<Animator>().SetBool("Poisoned", true);
    }

    protected override void EndCondition()
    {
        base.EndCondition();
        selfCombatant.Attributes.AttackSpeedMultiplier /= PoisonSpeedModifier;
        selfCombatant.Attributes.MovementSpeedMultiplier /= PoisonSpeedModifier;
        selfCombatant.GetComponent<Animator>().SetBool("Poisoned", false);
    }
}