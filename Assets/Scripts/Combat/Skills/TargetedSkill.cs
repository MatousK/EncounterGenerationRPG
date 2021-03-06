﻿using System;
using Assets.Scripts.Movement;
using Assets.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts.Combat.Skills
{
    /// <summary>
    /// Represents a skill that can target a single character.
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
        /// <summary>
        /// Object that can be used to indicate the direction of the attack.
        /// </summary>
        private AttackDirectionIndicator attackDirectionIndicator;
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void Start()
        {
            base.Start();
            attackDirectionIndicator = SelfCombatant.GetComponentInChildren<AttackDirectionIndicator>();
        }
        /// <summary>
        /// Stop using this skill if the current target cannot be targeted anymore for whatever reason.
        /// </summary>
        protected override void Update()
        {
            if (Target && !Target.CanBeTargeted)
            {
                TryStopSkill();
                return;
            }
            base.Update();
        }
        /// <summary>
        /// Returns the distance to the target of this skill.
        /// </summary>
        /// <returns>The distance to the target location, or max float value if the target is no longer set.</returns>
        public override float GetDistanceToTargetLocation()
        {
            if (Target == null)
            {
                // Sometimes target dies before reaching him.
                return float.MaxValue;
            }
            return SelfCombatant.GetComponent<Collider2D>().Distance(Target.GetComponent<Collider2D>()).distance;
        }
        /// <summary>
        /// Get the location of the target.
        /// </summary>
        /// <returns>Location of the target, or null if the target is dead.</returns>
        public override Vector2? GetTargetLocation()
        {
            if (Target == null)
            {
                return null;
            }
            return Target.transform.position;
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns><inheritdoc/></returns>
        public override bool IsBeingUsed()
        {
            return Target != null;
        }
        /// <summary>
        /// Called when this skill is activated with some specified target. Will start the execution of this skill.
        /// Cannot use the skill on himself.
        /// </summary>
        /// <param name="target">The target of this skill</param>
        /// <returns>True if this skill can be used on the specified target.</returns>
        public virtual bool UseSkillOn(CombatantBase target)
        {
            var toReturn = target != SelfCombatant && TryStartUsingSkill();
            if (toReturn)
            {
                Target = target;
            }
            return toReturn;
        }
        /// <summary>
        /// If true, we are currently moving to the target.
        /// </summary>
        /// <returns>True if we are moving to the target.</returns>
        public virtual bool IsMovingToTarget()
        {
            return !DidGetInRange;

        }
        /// <summary>
        /// Called when this skill should be stopped.
        /// Will clear the target if the skill can be stopped and stop movement towards that target.
        /// </summary>
        /// <returns>True if the movement can be stopped, otherwise false.</returns>
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
        /// <summary>
        /// Start the skill animation and show the attack indicator for this skill if necessary.
        /// </summary>
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
