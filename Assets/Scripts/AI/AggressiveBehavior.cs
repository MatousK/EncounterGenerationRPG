using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AggressiveBehavior : MonoBehaviour
{
    public float AggroRange = 5;
    PartyManager partyManager;
    AutoAttack autoAttack;

    private void Start()
    {
        partyManager = FindObjectOfType<PartyManager>();
        autoAttack = GetComponent<AutoAttack>();
    }
    // Update is called once per frame
    void Update()
    {
        if (!autoAttack.IsAutoAttacking)
        {
            AttackClosestCharacter();
        }
    }

    void AttackClosestCharacter()
    {
        var collider = GetComponent<Collider2D>();
        float closestDistance = float.PositiveInfinity;
        Character closestTarget = null;
        foreach (var character in partyManager.AlivePartyMembers)
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
            autoAttack.StartAutoAttack(closestTarget);
        }
    }
}
