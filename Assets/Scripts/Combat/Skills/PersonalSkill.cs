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
        /// <summary>
        /// How long should this skill stay active when activated.
        /// </summary>
        public float Duration = 1;
        /// <summary>
        /// If true, the skill is active right now
        /// </summary>
        protected bool IsActive;
        /// <summary>
        /// This method starts using this skill.
        /// </summary>
        public void ActivateSkill()
        {
            TryStartUsingSkill();
        }
        protected override void Start()
        {
            base.Start();
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
            // We do nothing by default, personal skills normally do something only when they start and end.
        }
        /// <summary>
        /// Try to start the skill. If successful, apply the personal skill effects, <see cref="OnPersonalSkillStarted"/>.
        /// </summary>
        /// <returns>True if this skill was started.</returns>
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
        /// <summary>
        /// Call when this skill should be stopped.
        /// If it can be stopped, undo the personal skill effects, see <see cref="OnPersonalSkillStopped"/>
        /// </summary>
        /// <returns>True if this skill was stopped, otherwise false.</returns>
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
        /// <summary>
        /// Courotine which stops this skill once its duration expires.
        /// </summary>
        /// <returns>The enumerator representing this courotine.</returns>
        private IEnumerator StopSkillTimer()
        {
            yield return new WaitForSeconds(Duration);
            TryStopSkill();
        }
    }
}
