using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Combat.Skills
{
    /// <summary>
    /// Represents a skill that affects only the character casting it.
    /// </summary>
    public abstract class PersonalSkill : Skill
    {
        protected PersonalSkill()
        {
            // Usually a personal skill does not block other skills
            BlocksOtherSkills = false;
            BlocksManualMovement = false;
            SkillAnimationName = "PersonalSkillAura";
        }
        public float Duration = 1;
        /// <summary>
        /// If true, the skill is active right now
        /// </summary>
        protected bool IsActive;
        public void ActivateSkill()
        {
            TryStartUsingSkill();
        }
        protected override void Awake()
        {
            base.Awake();
        }
        protected override void Update()
        {
            base.Update();
        }
        /// <summary>
        /// Returns distance to target of this skill. As personal skill has no range, this should always return 0.
        /// </summary>
        /// <returns>0, i.e. distance to the target</returns>
        public override float GetDistanceToTargetLocation()
        {
            return 0;
        }
        /// <summary>
        /// Gets the target location of this skill,used for moving toward the character and orientation. For personal skills, return null, we do not care about the target.
        /// </summary>
        /// <returns>Null.</returns>
        public override Vector2? GetTargetLocation()
        {
            return null;
        }
        public override bool IsBeingUsed()
        {
            return IsActive;
        }

        protected override void ApplySkillEffects(object sender, EventArgs e)
        {
            // We do nothing by default, auras work constantly.
        }

        protected override bool TryStartUsingSkill()
        {
            var startedSkill = base.TryStartUsingSkill();
            if (startedSkill)
            {
                IsActive = true;
                StartCoroutine(StopSkillTimer());
                OnPersonalSkillStarted();
            }
            return startedSkill;
        }

        public override bool TryStopSkill()
        {
            var stoppedSkill = base.TryStopSkill();
            if (stoppedSkill)
            {
                IsActive = false;
                OnPersonalSkillStopped();
            }
            return stoppedSkill;
        }
        /// <summary>
        /// Called when the the personal skill is activated.
        /// </summary>
        protected abstract void OnPersonalSkillStarted();
        /// <summary>
        /// Called when the personal skill is stopped.
        /// </summary>
        protected abstract void OnPersonalSkillStopped();

        private IEnumerator StopSkillTimer()
        {
            yield return new WaitForSeconds(Duration);
            TryStopSkill();
        }
    }
}
