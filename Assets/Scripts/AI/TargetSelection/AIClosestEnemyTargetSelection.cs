using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
/// <summary>
/// Chooses as its target the player that is the closest to the AI.
/// </summary>
class AIClosestEnemyTargetSelection : AITargetSelectionMethodBase
{
    CombatantsManager combatantsManager;
    protected override void Awake()
    {
        base.Awake();
        combatantsManager = FindObjectOfType<CombatantsManager>();
    }

    public override bool TryGetTarget(ref CombatantBase target)
    {
        float currentMinDistance = float.PositiveInfinity;
        foreach (var player in combatantsManager.GetPlayerCharacters(onlyAlive: true))
        {
            var distance = Vector2.Distance(player.transform.position, representedCombatant.transform.position);
            if (distance < currentMinDistance)
            {
                currentMinDistance = distance;
                target = player;
            }
        }
        return !float.IsPositiveInfinity(currentMinDistance);
    }
}