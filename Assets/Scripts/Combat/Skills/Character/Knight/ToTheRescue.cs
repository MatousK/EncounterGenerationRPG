using System;
using Assets.Scripts.Combat.Conditions;
using Assets.Scripts.Effects;
using Assets.Scripts.Movement;
using Assets.Scripts.Movement.Pathfinding;
using UnityEngine;

namespace Assets.Scripts.Combat.Skills.Character.Knight
{
    /// <summary>
    /// Moves quickly to the specified ally and taunts all nearby enemies, giving them the <see cref="ForcedTargetCondition"/>.
    /// </summary>
    public class ToTheRescue : TargetedSkill
    {
        /// <summary>
        /// Range of the taunt, i.e. how close must the enemies be to the ally to be affected.
        /// </summary>
        public float TauntRange = 3;
        /// <summary>
        /// Duration of the taunt.
        /// </summary>
        public float TauntDuration = 5;
        /// <summary>
        /// How much faster than usual will the knight move.
        /// </summary>
        public float MovementSpeedMultiplier = 5;
        /// <summary>
        /// An object which holds information about which squares are passable and which are not.
        /// </summary>
        private PathfindingMapController pathfindingMapController;
        /// <summary>
        /// An object which knows about all combatants in the game.
        /// </summary>
        private CombatantsManager combatantsManager;
        /// <summary>
        /// The grid where the game is taking place.
        /// </summary>
        private Grid mapGrid;
        /// <summary>
        /// Initializes a new instance of the <see cref="ToTheRescue"/> class.
        /// </summary>
        public ToTheRescue()
        {
            Speed = 2;
            Range = float.MaxValue;
            SkillAnimationName = "Walking";
        
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void Start()
        {
            base.Start();
            pathfindingMapController = FindObjectOfType<PathfindingMapController>();
            mapGrid = FindObjectOfType<Grid>();
            combatantsManager = FindObjectOfType<CombatantsManager>();
        }
        /// <summary>
        /// <inheritdoc/> Will only work if there is an empty space near the ally.
        /// </summary>
        /// <param name="target">Target of the skill.</param>
        /// <returns>True of the skill can be used, otherwise false.</returns>
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

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void Update()
        {
            base.Update();
        }
        /// <summary>
        /// <inheritdoc/>. Does not call base on purpose, as it starts working immediately.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void ApplySkillEffects(object sender, EventArgs e) 
        {
            // Do nothing, as this skill works differently fron other ones and is independent on animations.
        }
        /// <summary>
        /// Starts using the skill. This means increasing movement speed and moving to the target square.
        /// </summary>
        /// <param name="targetPosition">The square near the ally.</param>
        private void ExecuteSkill(Vector2Int targetPosition)
        {
            SelfCombatant.Attributes.MovementSpeedMultiplier *= MovementSpeedMultiplier;
            Vector2 targetWorldSpace = mapGrid.CellToWorld((Vector3Int)targetPosition);
            SelfCombatant.GetComponent<MovementController>().MoveToPosition(targetWorldSpace, ignoreOtherCombatants: true, onMoveToSuccessful: MoveToSuccessful);
        }
        /// <summary>
        /// Called when the knight finishes his movement. Restores movement speed and applies taunt to the nearby allow using <see cref="ForcedTargetCondition"/>. 
        /// After this the skill is stopped.
        /// </summary>
        /// <param name="result">True if the movement was successful.</param>
        private void MoveToSuccessful(bool result)
        {
            SelfCombatant.Attributes.MovementSpeedMultiplier /= MovementSpeedMultiplier;
            SelfCombatant.GetComponentInChildren<TauntEffect>().StartEffect();
            foreach (var enemy in combatantsManager.GetEnemies(onlyAlive: true))
            {
                if (Vector2.Distance(enemy.transform.position, SelfCombatant.transform.position) > TauntRange)
                {
                    continue;
                }
                var tauntCondition = enemy.GetComponent<ConditionManager>().AddCondition<ForcedTargetCondition>();
                tauntCondition.RemainingDuration = TauntDuration;
                tauntCondition.ForcedTarget = SelfCombatant;
                tauntCondition.TargetForcedBy = SelfCombatant;
            }
            TryStopSkill();
        }
        /// <summary>
        /// Finds a valid space around the target, i.e. an empty passable space.
        /// </summary>
        /// <param name="target">Target of the skill.</param>
        /// <returns>Null if there are no empty spaces.</returns>
        private Vector2Int? GetTargetSquare(CombatantBase target)
        {
            return pathfindingMapController.GetPassableSpaceInDistance(SelfCombatant, (Vector2Int)mapGrid.WorldToCell(target.transform.position), 1);
        }
        /// <summary>
        /// <inheritdoc/> Does not call base, as this skill works in a unique way.
        /// </summary>
        /// <param name="sender">Sender of this event.</param>
        /// <param name="e">Arguments of this event.</param>
        protected override void AnimationCompleted(object sender, EventArgs e)
        {
            // do nothing, we just do not want the base implementation to stop the skill prematurely.
        }
        /// <summary>
        /// Does nothing, does not call base, this skill does not use skill animations like normal skills.
        /// </summary>
        protected override void StartSkillAnimation()
        {
            // Do nothing, we do not want to call the base implementation which stops movement. Also, we really do not have a skill animation.
        }
    }
}
