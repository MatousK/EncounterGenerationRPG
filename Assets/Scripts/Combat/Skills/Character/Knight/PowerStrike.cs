using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// A quick strike that knocks back an enemy and momentarily stuns him.
/// Also taunts the enemy for a short while.
/// </summary>
public class PowerStrike : Attack
{
    /// <summary>
    /// How long should the target be stunned after this attack.
    /// </summary>
    public float StunDuration;
    /// <summary>
    /// How long should the target be taunted after this attack.
    /// </summary>
    public float TauntDuration;
    /// <summary>
    /// How far should this attack knockback the enemy.
    /// </summary>
    public int KnockbackDistance;
    private PathfindingMapController pathfindingMapController;
    private Grid mapGrid;
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        pathfindingMapController = FindObjectOfType<PathfindingMapController>();
        mapGrid = FindObjectOfType<Grid>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected override void ApplySkillEffects(object sender, EventArgs e)
    {
        base.ApplySkillEffects(sender, e);
        var stunCondition = Target.GetComponent<ConditionManager>().AddCondition<StunCondition>();
        stunCondition.RemainingDuration = StunDuration;
        var tauntCondition = Target.GetComponent<ConditionManager>().AddCondition<ForcedTargetCondition>();
        tauntCondition.RemainingDuration = TauntDuration;
        tauntCondition.ForcedTarget = selfCombatant;
        tauntCondition.TargetForcedBy = selfCombatant;
        KnockbackEnemy();

    }

    private void KnockbackEnemy()
    {
        Vector2 target2dPosition = Target.transform.position;
        Vector2 knocbackDirectionVector = Target.transform.position - transform.position;
        knocbackDirectionVector.Normalize();
        var passabilityMap = pathfindingMapController.GetPassabilityMapForCombatant(Target);
        for (int i = KnockbackDistance; i > 0; --i)
        {
            var potentialTarget = target2dPosition + (knocbackDirectionVector * i);
            Vector2Int potentialTargetGridSpace = (Vector2Int)mapGrid.WorldToCell(potentialTarget);
            if (passabilityMap.GetSquareIsPassable(potentialTargetGridSpace))
            {
                var targetOrientationController = Target.GetComponent<OrientationController>();
                targetOrientationController.LockOrientation = true;
                Target.GetComponent<MovementController>().MoveToPosition(potentialTarget, ignoreOtherCombatants: true, animate: false, onMoveToSuccessful: (_) => targetOrientationController.LockOrientation = false);
                break;
            }
        }
    }
}
