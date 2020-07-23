using System;
using Assets.Scripts.Effects;
using Assets.Scripts.Movement.Pathfinding;
using UnityEngine;

namespace Assets.Scripts.Combat.Skills.Monster.Lurker
{
    /// <summary>
    ///  A skill that teleports the lurker to a square near the target.
    /// </summary>
    class FireTeleport : TargetedGestureSkill
    {
        /// <summary>
        /// A grid representing the target area.
        /// </summary>
        Grid mapGrid;
        /// <summary>
        /// A class controlling pathfinding. Most importantly, it knows which squares are passable for this combatant and which are not.
        /// </summary>
        PathfindingMapController pathfindingMapController;
        /// <summary>
        /// The effect of the fire swallowing the lurker.
        /// </summary>
        FireTeleportEffect fireTeleportEffect;
        /// <summary>
        /// If true, the fire animation has started.
        /// </summary>
        bool startedFireAnimation = false;
        /// <summary>
        /// If true, the lurker has disappeared.
        /// </summary>
        bool isDisappeared;
        FireTeleport()
        {
            //Lurkers rule and can teleport wherever they want.
            Range = float.MaxValue;
        }

        protected override void Start()
        {
            base.Start();
            fireTeleportEffect = SelfCombatant.GetComponentInChildren<FireTeleportEffect>();
            fireTeleportEffect.OnFireAnimationEnded += FireAnimationEnded;
            fireTeleportEffect.OnFireMaxSize += FireAnimationMaxSize;
            pathfindingMapController = FindObjectOfType<PathfindingMapController>();
            mapGrid = FindObjectOfType<Grid>();
        }
        /// <summary>
        /// Called when the skill is started. Resets the internal state of the skill.
        /// </summary>
        /// <returns>True if the skill could be started. Otherwise false.</returns>
        protected override bool TryStartUsingSkill()
        {
            var toReturn = base.TryStartUsingSkill();
            if (toReturn)
            {
                startedFireAnimation = false;
            }
            return toReturn;
        }
        /// <summary>
        /// When the lurker finishes the casting animation, the fire animation will start and swallow him.
        /// </summary>
        /// <param name="sender">The sender of this event.</param>
        /// <param name="e">Arguments of the event.</param>
        protected override void ApplySkillEffects(object sender, EventArgs e)
        {
            if (!startedFireAnimation)
            {
                fireTeleportEffect.StartFire();
                startedFireAnimation = true;
            }
            base.ApplySkillEffects(sender, e);
        }
        /// <summary>
        /// This does nothing. It does not even call the base behavior, because that could stop the skill. We want it to stop once the teleport finishes.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Arguments of this event.</param>
        protected override void AnimationCompleted(object sender, EventArgs e) { }
        /// <summary>
        /// Called when the fire animation reaches its max size. 
        /// This will be called twice. Once when the flames swallow the lurker and he disappears.
        /// The second time it will be when the fire appears at the target location. In this case the lurker should appear.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Arguments of the event</param>
        private void FireAnimationMaxSize(object sender, EventArgs e)
        {
            // While teleporting the lurker is invincible. Lurker dying while teleporting was causing some bugs and was confusing.
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
        /// <summary>
        /// Called when the fire effect reaches its minimal size once more. Will be called twice.
        /// First time it will move the lurker.
        /// Second time it will stop this skill.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                else
                {
                    // This should not happen, but we will just appear the next time the fire animation finishes.
                    UnityEngine.Debug.Assert(false, "Could not find teleport target square");
                }
            }
            else
            {
                TryStopSkill();
            }
        }
        /// <summary>
        /// Find the closest space to the target to teleport to.
        /// </summary>
        /// <returns>The closest space.</returns>
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
