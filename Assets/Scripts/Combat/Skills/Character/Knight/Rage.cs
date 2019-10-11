using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Rage : PersonalSkill
{
    protected int RageDuration = 5;
    protected int RageDamageMultiplier = 4;
    protected float RageSpeedMultiplier = 4;
    protected float RageCooldown = 10;

    CombatantsManager combatantsManager;
    SelectableObject selectableComponent;
    CombatantBase combatant;

    public Rage()
    {
        Duration = RageDuration;
        SkillAnimationName = "Raging";
        Cooldown = RageCooldown;
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
    protected override void Start()
    {
        combatant = GetComponent<CombatantBase>();
        combatantsManager = FindObjectOfType<CombatantsManager>();
        selectableComponent = GetComponent<SelectableObject>();
        base.Start();
    }
    protected override void OnPersonalSkillStarted()
    {
        if (selectableComponent != null)
        {
            selectableComponent.IsSelected = false;
            selectableComponent.IsSelectionEnabled = false;
        }

        combatant.Attributes.MovementSpeedMultiplier *= RageSpeedMultiplier;
        combatant.Attributes.AttackSpeedMultiplier *= RageSpeedMultiplier;
        combatant.Attributes.DealtDamageMultiplier *= RageDamageMultiplier;
    }

    protected override void OnPersonalSkillStopped()
    {
        if (selectableComponent != null)
        {
            selectableComponent.enabled = true;
            selectableComponent.IsSelectionEnabled = true;
        }

        combatant.Attributes.MovementSpeedMultiplier /= RageSpeedMultiplier;
        combatant.Attributes.AttackSpeedMultiplier /= RageSpeedMultiplier;
        combatant.Attributes.DealtDamageMultiplier /= RageDamageMultiplier;
    }
}
