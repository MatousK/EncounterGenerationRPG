using System;
using Assets.Scripts.Movement;
using Assets.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts.Combat.Skills
{
    /// <summary>
    /// Represents a skill that can target a single character and has animations.
    /// </summary>
    public abstract class TargetedSkill : Skill
    {
        /// <summary>
        /// If true, a sword icon will be displayed and will point towards the target.
        /// </summary>
        public bool ShouldShowAttackIndicator = true;
        /// <summary>
        /// How many times does the animation repeat as part of one skill usage.
        /// </summary>
        public int Repetitions = 1;
        /// <summary>
        /// Target which we are currently using this skill on.
        /// </summary>
        public CombatantBase Target { get; protected set; }

        private AttackDirectionIndicator attackDirectionIndicator;
        protected override void Start()
        {
            base.Start();
            attackDirectionIndicator = SelfCombatant.GetComponentInChildren<AttackDirectionIndicator>();
        }

        protected override void Update()
        {
            if (Target && !Target.CanBeTargeted)
            {
                TryStopSkill();
                return;
            }
            base.Update();
        }

        public override float GetDistanceToTargetLocation()
        {
            if (Target == null)
            {
                // Sometimes target dies before reaching him.
                return float.MaxValue;
            }
            return SelfCombatant.GetComponent<Collider2D>().Distance(Target.GetComponent<Collider2D>()).distance;
        }

        public override Vector2? GetTargetLocation()
        {
            if (Target == null)
            {
                return null;
            }
            return Target.transform.position;
        }

        public override bool IsBeingUsed()
        {
            return Target != null;
        }

        public virtual bool UseSkillOn(CombatantBase target)
        {
            var toReturn = target != SelfCombatant && TryStartUsingSkill();
            if (toReturn)
            {
                Target = target;
            }
            return toReturn;
        }

        public virtual bool IsMovingToTarget()
        {
            return !DidGetInRange;

        }

        public override bool TryStopSkill()
        {
            var didStopSkill = base.TryStopSkill();
            if (didStopSkill)
            {
                Target = null;
                // Stop trying to move to position if relevant.
                SelfCombatant.GetComponent<MovementController>().StopMovement();
            }
            return didStopSkill;
        }

        protected override void StartSkillAnimation()
        {
            if (ShouldShowAttackIndicator)
            {
                if (attackDirectionIndicator != null)
                {
                    attackDirectionIndicator.IndicateAttackOnTarget(Target);
                }
            }

            base.StartSkillAnimation();
        }

        /// <summary>
        /// Called when the skill animation completes.
        /// Default implementation will stop using the skill if this method was called sufficient amount of times, <see cref="Repetitions"/>
        /// </summary>
        protected override void AnimationCompleted(object sender, EventArgs e)
        {
            AnimationCompletedCount++;
            if (AnimationCompletedCount >= Repetitions)
            {
                TryStopSkill();
            }
        }
    }
}
