using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class FireTeleport : TargetedGestureSkill
{
    Grid mapGrid;
    PathfindingMapController pathfindingMapController;
    FireTeleportEffect fireTeleportEffect;
    bool startedFireAnimation = false;
    bool isDisappeared;
    FireTeleport()
    {
        Range = float.MaxValue;
    }

    protected override void Awake()
    {
        base.Awake();
        fireTeleportEffect = selfCombatant.GetComponentInChildren<FireTeleportEffect>();
        fireTeleportEffect.OnFireAnimationEnded += FireAnimationEnded;
        fireTeleportEffect.OnFireMaxSize += FireAnimationMaxSize;
        pathfindingMapController = FindObjectOfType<PathfindingMapController>();
        mapGrid = FindObjectOfType<Grid>();
    }

    protected override bool TryStartUsingSkill()
    {
        var toReturn = base.TryStartUsingSkill();
        if (toReturn)
        {
            startedFireAnimation = false;
        }
        return toReturn;
    }

    protected override void ApplySkillEffects(object sender, EventArgs e)
    {
        if (!startedFireAnimation)
        {
            fireTeleportEffect.StartFire();
            startedFireAnimation = true;
        }
    }

    protected override void AnimationCompleted(object sender, EventArgs e)
    {
        // Do nothing, we just do not want the default behavior of stopping the skill, the skill should stop when we tell it to.
    }

    private void FireAnimationMaxSize(object sender, EventArgs e)
    {
        selfCombatant.IsInvincible = !selfCombatant.IsInvincible;
        isDisappeared = !isDisappeared;
        // Disable or enable all sprite renderers except for the fire effect.
        foreach (var spriteRenderer in selfCombatant.GetComponentsInChildren<SpriteRenderer>())
        { 
            if (spriteRenderer.GetComponent<FireTeleportEffect>() != null)
            {
                continue;
            }
            spriteRenderer.enabled = !isDisappeared;
        }
        selfCombatant.GetComponent<Animator>().enabled = !isDisappeared;
    }

    protected override void Update()
    {
        base.Update();
    }

    private void FireAnimationEnded(object sender, EventArgs e)
    {
        if (isDisappeared)
        {
            // Do the teleport.
            var targetSquare = GetTargetSquare();
            if (targetSquare != null)
            {
                var targetSquareWorldSpace = mapGrid.CellToWorld((Vector3Int)targetSquare.Value);
                selfCombatant.transform.position = new Vector3(targetSquareWorldSpace.x, targetSquareWorldSpace.y, selfCombatant.transform.position.z);
            }
        }
        else
        {
            TryStopSkill();
        }
    }

    private Vector2Int? GetTargetSquare()
    {
        // TODO: Make sure not to get out of bounds... Probably different handling will be required.
        return pathfindingMapController.GetPassableSpaceInDistance(selfCombatant, (Vector2Int)mapGrid.WorldToCell(Target.transform.position), 100);
    }
}
