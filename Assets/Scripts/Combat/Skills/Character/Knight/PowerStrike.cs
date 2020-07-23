using System;
using Assets.Scripts.Combat.Conditions;
using Assets.Scripts.Movement;
using Assets.Scripts.Movement.Pathfinding;
using UnityEngine;

namespace Assets.Scripts.Combat.Skills.Character.Knight
{
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
        /// <summary>
        /// An object which knows about the pathfinding map, of the game.
        /// </summary>
        private PathfindingMapController pathfindingMapController;
        /// <summary>
        /// The grid representing the game map.
        /// </summary>
        private Grid mapGrid;
        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            pathfindingMapController = FindObjectOfType<PathfindingMapController>();
            mapGrid = FindObjectOfType<Grid>();
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
        }
        /// <summary>
        /// <inheritdoc/> Gives the enemy stun and forced target conditions. and knocks him back.
        /// <see cref="StunCondition"/>
        /// <see cref="ForcedTargetCondition"/>
        /// <see cref="KnockbackEnemy"/>
        /// </summary>
        /// <param name="sender">Sender of this event.</param>
        /// <param name="e">Arguments of this event.</param>
        protected override void ApplySkillEffects(object sender, EventArgs e)
        {
            base.ApplySkillEffects(sender, e);
            var stunCondition = Target.GetComponent<ConditionManager>().AddCondition<StunCondition>();
            stunCondition.RemainingDuration = StunDuration;
            var tauntCondition = Target.GetComponent<ConditionManager>().AddCondition<ForcedTargetCondition>();
            tauntCondition.RemainingDuration = TauntDuration;
            tauntCondition.ForcedTarget = SelfCombatant;
            tauntCondition.TargetForcedBy = SelfCombatant;
            KnockbackEnemy();

        }
        /// <summary>
        /// Tries to knocback an enemy. Will go through spaces behind the target from the opposite direction than from which the knight is attacking.
        /// Find the furthest square in the opposite direction than the knight and moves him there. Will not move the enemy more then specified distance,
        /// <see cref="KnockbackDistance"/>
        /// </summary>
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
}
