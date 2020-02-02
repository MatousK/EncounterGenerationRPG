using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Rage : PersonalSkill
{
    public int DamageMultiplier = 4;
    public float SpeedMultiplier = 4;

    CombatantsManager combatantsManager;
    SelectableObject selectableComponent;

    public Rage()
    {
        Duration = 5;
        Cooldown = 10;
    }

    protected override void Update()
    {
        base.Update();
        if (IsActive && !combatantsManager.GetOpponentsFor(selfCombatant, onlyAlive:true).Any())
        {
            // Everyone is dead, noone to attack.
            TryStopSkill();
        }
    }
    protected override void Awake()
    {
        base.Awake();
        combatantsManager = FindObjectOfType<CombatantsManager>();
        selectableComponent = selfCombatant.GetComponent<SelectableObject>();
    }
    protected override void OnPersonalSkillStarted()
    {
        if (selectableComponent != null)
        {
            selectableComponent.IsSelected = false;
            selectableComponent.IsSelectionEnabled = false;
        }

        selfCombatant.Attributes.MovementSpeedMultiplier *= SpeedMultiplier;
        selfCombatant.Attributes.AttackSpeedMultiplier *= SpeedMultiplier;
        selfCombatant.Attributes.DealtDamageMultiplier *= DamageMultiplier;
    }

    protected override void OnPersonalSkillStopped()
    {
        if (selectableComponent != null)
        {
            selectableComponent.enabled = true;
            selectableComponent.IsSelectionEnabled = true;
        }

        selfCombatant.Attributes.MovementSpeedMultiplier /= SpeedMultiplier;
        selfCombatant.Attributes.AttackSpeedMultiplier /= SpeedMultiplier;
        selfCombatant.Attributes.DealtDamageMultiplier /= DamageMultiplier;
    }
}
