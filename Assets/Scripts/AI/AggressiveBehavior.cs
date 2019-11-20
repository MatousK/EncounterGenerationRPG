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
        if (autoAttacking.Target == null && !selfCombatant.IsDoingNonAutoAttackAction())
        {
            AttackClosestOpponent();
        }
    }

    void AttackClosestOpponent()
    {
        var collider = GetComponent<Collider2D>();
        float closestDistance = float.PositiveInfinity;
        CombatantBase closestTarget = null;
        foreach (var opponent in combatantsManager.GetOpponentsFor(selfCombatant, onlyAlive: true))
        {
            var distanceToOpponent = opponent.GetComponent<Collider2D>().Distance(collider).distance;
            if (distanceToOpponent < closestDistance)
            {
                closestDistance = distanceToOpponent;
                closestTarget = opponent;
            }
        }
        if (closestDistance < AggroRange)
        {
            autoAttacking.Target = closestTarget;
        }
    }
}
