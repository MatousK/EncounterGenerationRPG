using System;
using Assets.Scripts.Effects;
using Assets.Scripts.Movement.Pathfinding;
using UnityEngine;

namespace Assets.Scripts.Combat.Skills.Monster.Lurker
{
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
            fireTeleportEffect = SelfCombatant.GetComponentInChildren<FireTeleportEffect>();
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
            base.ApplySkillEffects(sender, e);
        }

        protected override void AnimationCompleted(object sender, EventArgs e)
        {
            // Do nothing, we just do not want the default behavior of stopping the skill, the skill should stop when we tell it to.
        }

        private void FireAnimationMaxSize(object sender, EventArgs e)
        {
            SelfCombatant.IsInvincible = !SelfCombatant.IsInvincible;
            isDisappeared = !isDisappeared;
            // Disable or enable all sprite renderers except for the fire effect.
            foreach (var spriteRenderer in SelfCombatant.GetComponentsInChildren<SpriteRenderer>())
            { 
                if (spriteRenderer.GetComponent<FireTeleportEffect>() != null)
                {
                    continue;
                }
                spriteRenderer.enabled = !isDisappeared;
            }
            SelfCombatant.GetComponent<Animator>().enabled = !isDisappeared;
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
                    SelfCombatant.transform.position = new Vector3(targetSquareWorldSpace.x, targetSquareWorldSpace.y, SelfCombatant.transform.position.z);
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
            if (Target == null || SelfCombatant == null)
            {
                return null;
            }
            return pathfindingMapController.GetPassableSpaceInDistance(SelfCombatant, (Vector2Int)mapGrid.WorldToCell(Target.transform.position), 100);
        }
    }
}
