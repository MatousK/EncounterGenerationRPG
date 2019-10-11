using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AggressiveBehavior : MonoBehaviour
{
    public float AggroRange = 5;
    CombatantsManager combatantsManager;
    AutoAttacking autoAttacking;
    CombatantBase selfCombatant;

    private void Start()
    {
        combatantsManager = FindObjectOfType<CombatantsManager>();
        autoAttacking = GetComponent<AutoAttacking>();
        selfCombatant = GetComponent<CombatantBase>();
    }
    // Update is called once per frame
    void Update()
    {
        if (GetComponent<Rage>()?.CanUseSkill() == true)
        {
            GetComponent<Rage>().ActivateSkill();
        }
        if (autoAttacking.Target == null)
        {
            AttackClosestOpponent();
        }
    }

    void AttackClosestOpponent()
    {
        var collider = GetComponent<Collider2D>();
        float closestDistance = float.PositiveInfinity;
        CombatantBase closestTarget = null;
        foreach (var character in combatantsManager.GetOpponentsFor(selfCombatant, onlyAlive: true))
        {
            var distanceToCharacter = character.GetComponent<Collider2D>().Distance(collider).distance;
            if (distanceToCharacter < closestDistance)
            {
                closestDistance = distanceToCharacter;
                closestTarget = character;
            }
        }
        if (closestDistance < AggroRange)
        {
            autoAttacking.Target = closestTarget;
        }
    }
}
