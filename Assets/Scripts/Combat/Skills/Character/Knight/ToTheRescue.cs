using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToTheRescue : TargetedSkill
{
    public float TauntRange = 3;
    public float TauntDuration = 5;
    public float MovementSpeedMultiplier = 5;
    private PathfindingMapController pathfindingMapController;
    private CombatantsManager combatantsManager;
    private Grid mapGrid;
    public ToTheRescue()
    {
        Speed = 2;
        Range = float.MaxValue;
        SkillAnimationName = "Walking";
        
    }

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        pathfindingMapController = FindObjectOfType<PathfindingMapController>();
        mapGrid = FindObjectOfType<Grid>();
        combatantsManager = FindObjectOfType<CombatantsManager>();
    }

    public override bool UseSkillOn(CombatantBase target)
    {
        var targetSquare = GetTargetSquare(target);
        if (targetSquare == null)
        {
            return false;
        }
        // Also check other preconditions in parent , like cooldown etc.
        var toReturn = base.UseSkillOn(target);
        if (toReturn)
        {
            ExecuteSkill(targetSquare.Value);
        }
        return toReturn;
    }

    // Update is called once per frame
    protected override void Update()
    {
        // Do not call the standard update loop, this skill has unique handling.
    }

    protected override void ApplySkillEffects(object sender, EventArgs e) 
    {
        // Do nothing, as this skill works differently fron other ones and is independent on animations.
    }

    private void ExecuteSkill(Vector2Int targetPosition)
    {
        selfCombatant.Attributes.MovementSpeedMultiplier *= MovementSpeedMultiplier;
        Vector2 targetWorldSpace = mapGrid.CellToWorld((Vector3Int)targetPosition);
        selfCombatant.GetComponent<MovementController>().MoveToPosition(targetWorldSpace, ignoreOtherCombatants: true, onMoveToSuccessful: MoveToSuccessful);
    }

    private void MoveToSuccessful(bool result)
    {
        selfCombatant.Attributes.MovementSpeedMultiplier /= MovementSpeedMultiplier;
        selfCombatant.GetComponentInChildren<TauntEffect>().StartEffect();
        foreach (var enemy in combatantsManager.GetEnemies(onlyAlive: true))
        {
            if (Vector2.Distance(enemy.transform.position, selfCombatant.transform.position) > TauntRange)
            {
                continue;
            }
            var tauntCondition = enemy.GetComponent<ConditionManager>().AddCondition<ForcedTargetCondition>();
            tauntCondition.RemainingDuration = TauntDuration;
            tauntCondition.ForcedTarget = selfCombatant;
            tauntCondition.TargetForcedBy = selfCombatant;
        }
        TryStopSkill();
    }

    private Vector2Int? GetTargetSquare(CombatantBase target)
    {
        return pathfindingMapController.GetPassableSpaceInDistance(selfCombatant, (Vector2Int)mapGrid.WorldToCell(target.transform.position), 1);
    }

    protected override void AnimationCompleted(object sender, EventArgs e)
    {
        // do nothing, we just do not want the base implementation to stop the skill prematurely.
    }
}
