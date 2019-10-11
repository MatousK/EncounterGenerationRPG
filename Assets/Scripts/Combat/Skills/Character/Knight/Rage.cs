using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rage : PersonalSkill
{
    const int RageDuration = 5;
    const int RageDamageMultiplier = 4;
    const float RageSpeedMultiplier = 4;
    const float RageCooldown = 10;

    AutoAttacking autoAttacking;
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
        if (IsActive && autoAttacking.Target == null)
        {
            AutoAttackClosestEnemy();
        }
    }
    protected override void Start()
    {
        combatant = GetComponent<CombatantBase>();
        autoAttacking = GetComponent<AutoAttacking>();
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
        AutoAttackClosestEnemy();

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

    private void AutoAttackClosestEnemy()
    {
        float closestDistance = float.PositiveInfinity;
        CombatantBase closestTarget = null;
        foreach (var target in combatantsManager.GetOpponentsFor(selfCombatant))
        {
            var distanceToTarget = target.GetComponent<Collider2D>().Distance(GetComponent<Collider2D>()).distance;
            if (distanceToTarget < closestDistance)
            {
                closestDistance = distanceToTarget;
                closestTarget = target;
            }
        }
        if (closestTarget == null)
        {
            TryStopSkill();
        }
        else
        {
            autoAttacking.Target = closestTarget;
        }
    }
}
