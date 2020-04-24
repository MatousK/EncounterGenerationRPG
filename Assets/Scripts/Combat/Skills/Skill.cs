using System;
using Assets.Scripts.Animations;
using Assets.Scripts.Movement;
using Assets.Scripts.Sound.CharacterSounds;
using Assets.Scripts.Sound.CombatSounds;
using UnityEngine;

namespace Assets.Scripts.Combat.Skills
{
    /// <summary>
    /// Represents a skill a character can use.
    /// </summary>
    public abstract class Skill: MonoBehaviour
    {
        /// <summary>
        /// Icon representing this skill in the user interface.
        /// </summary>
        public Sprite SkillIcon;
        /// <summary>
        /// The name of the skill to be shown in the user interface.
        /// </summary>
        public string SkillName;
        /// <summary>
        /// The description of the skill to be shown in the user interface.
        /// </summary>
        public string SkillDescription;
        /// <summary>
        /// If true, after using this skill the target of the combatant will be cleared no matter what else.
        /// E.g. after putting an enemy to sleep, we do not want to keep attacking him.
        /// </summary>
        public bool ClearTargetAfterUsingSkill = false;
        /// <summary>
        /// If true, this attack is considered to be a basic attack for some purposes, like auto attacking.
        /// </summary>
        [NonSerialized]
        public bool IsBasicAttack;
        /// <summary>
        /// If true, the player cannot start another skill while this skill is being executed.
        /// </summary>
        [NonSerialized]
        public bool BlocksOtherSkills = true;
        /// <summary>
        /// If true, the player cannot order the character to move while this skill is being executed.
        /// </summary>
        [NonSerialized]
        public bool BlocksManualMovement = true;
        /// <summary>
        /// Number of seconds for which the skill cannot be used again once it's used.
        /// </summary>
        public float Cooldown;
        /// <summary>
        /// How many spaces away from target can the character be to start using the skill.
        /// </summary>
        public float Range = 1f;
        /// <summary>
        /// How fast should the animation play. 1 is start, 2 is twice as fast, 0.5 is half as slow etc.
        /// </summary>
        public float Speed = 1;
        /// <summary>
        /// The type of sound this skill will make.
        /// </summary>
        public CombatSoundType SkillSoundType = CombatSoundType.None;
        /// <summary>
        /// The type of sound that should be played for this skill.
        /// </summary>
        public SkillVoiceType VoiceSkillType = SkillVoiceType.SkillNormal;
        /// <summary>
        /// The name of the animation that should be played while this skill is being used.
        /// </summary>
        [NonSerialized]
        public string SkillAnimationName;
        /// <summary>
        /// This should fire events when a skill animation reaches points when we should react to it.
        /// </summary>
        protected AnimationEventsListener AnimationEventListener;
        /// <summary>
        /// If true, this character already moved in range and is free to continue using the skill regardless of range.
        /// </summary>
        protected bool DidGetInRange;
        /// <summary>
        /// If true, this skill is being used right now.
        /// </summary>
        protected bool IsUsingSkill;
        /// <summary>
        /// How many times was this animation already completed while using this skill
        /// </summary>
        protected int AnimationCompletedCount;
        /// <summary>
        /// Combatant who is using this skill.
        /// </summary>
        protected CombatantBase SelfCombatant;
        /// <summary>
        /// Class responsible for playing sounds when the skill hits specified moments.
        /// </summary>
        protected CombatSoundsController CombatSoundsController;
        /// <summary>
        /// Can play a sound for a specific character
        /// </summary>
        protected CharacterVoiceController CharacterVoiceController;
        // Start is called before the first frame update
        protected virtual void Start()
        {
            // First, travel the tree to find the combatant object.
            SelfCombatant = GetComponentInParent<CombatantBase>();
            AnimationEventListener = SelfCombatant.GetComponent<AnimationEventsListener>();
            CombatSoundsController = SelfCombatant.GetComponentInChildren<CombatSoundsController>();
            CharacterVoiceController = SelfCombatant.GetComponentInChildren<CharacterVoiceController>();
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            if (!IsBeingUsed())
            {
                return;
            }
            var rangeMultiplier = SelfCombatant.Attributes.RangeMultiplier;
            if (GetTargetLocation() != null && !DidGetInRange && GetDistanceToTargetLocation() > Range * rangeMultiplier)
            {
                if (SelfCombatant. GetComponent<MovementController>().IsMoving)
                {
                    // Already moving somewhere do not start another move.
                    return;
                }
                // Move in range.
                SelfCombatant.GetComponent<MovementController>().MoveToPosition(GetTargetLocation().Value, onMoveToSuccessful:(result) =>
                {
                    // Probably cannot reach the target.
                    if (GetDistanceToTargetLocation() > Range * rangeMultiplier)
                    {
                        TryStopSkill();
                    }
                });
                return;
            }
            DidGetInRange = true;
            if (!IsUsingSkill)
            {
                IsUsingSkill = true;
                StartSkillAnimation();
            }
        }
        public abstract float GetDistanceToTargetLocation();
        /// <summary>
        /// The location of the target to move towards and orient towards. Return null if we do not care about either of those things.
        /// </summary>
        /// <returns>The location of the target, or null if we do not wish to orient ourselves toward the target.</returns>
        public abstract Vector2? GetTargetLocation();
        public abstract bool IsBeingUsed();
        /// <summary>
        /// Call to start execution of this skill.
        /// </summary>
        /// <returns>True if the skill cnan be used right now, otherwise false.</returns>
        protected virtual bool TryStartUsingSkill()
        {
            if (!CanUseSkill())
            {
                // Already using the skill on someone or we're in cooldown.
                return false;
            }
            if (Cooldown > 0)
            {
                SelfCombatant.StartCooldown(Cooldown);
            }
            DidGetInRange = false;
            IsUsingSkill = false;
            AnimationCompletedCount = 0;
            AnimationEventListener.ApplySkillEffect += ApplySkillEffects;
            AnimationEventListener.SkillAnimationFinished += AnimationCompleted;
            return true;
        }
        /// <summary>
        /// Call to stop using this skill.
        /// </summary>
        /// <returns>True if the skill can be stopped right now.</returns>
        public virtual bool TryStopSkill()
        {
            if (!IsBeingUsed())
            {
                // Skill is not being used right now, nothing to stop.
                return false;
            }
            if (!string.IsNullOrEmpty(SkillAnimationName))
            {
                SelfCombatant.GetComponent<Animator>().SetBool(SkillAnimationName, false);
            }
            SelfCombatant.GetComponent<OrientationController>().LookAtTarget = null;
            // HACK: Auto attacking uses the same animation as attack skills.
            // If an attack skill interrupted a basic attack, the animation would not reset, leading to bugs.
            // This solution is not ideal, it would cease working if someone used for example gesture for basic attacks.
            SelfCombatant.GetComponent<Animator>().Play("Attack", -1, 0);
            AnimationEventListener.ApplySkillEffect -= ApplySkillEffects;
            AnimationEventListener.SkillAnimationFinished -= AnimationCompleted;
            return true;
        }
        // Return true if this skill can be used at this moment.
        public bool CanUseSkill()
        {
            return !IsBeingUsed() && (IsBasicAttack || ( SelfCombatant.LastSkillRemainingCooldown ?? 0) <= 0);
        }

        /// <summary>
        /// Called when the skill animation hits the point where the effects should be applied
        /// </summary>
        protected virtual void ApplySkillEffects(object sender, EventArgs e)
        {
            CharacterVoiceController.SkillAnimationEffectApplied(VoiceSkillType);
            CombatSoundsController.AnimationEffectApplied(SkillSoundType);
        }
        /// <summary>
        /// Called when the skill animation completes.
        /// </summary>
        protected virtual void AnimationCompleted(object sender, EventArgs e)
        {
            CombatSoundsController.AnimationEnded(SkillSoundType);
        }

        protected virtual void StartSkillAnimation()
        {
            CharacterVoiceController.SkillAnimationStartedStarted(VoiceSkillType);
            CombatSoundsController.AnimationStarted(SkillSoundType);
            SelfCombatant.GetComponent<MovementController>().StopMovement();
            // In range, start using the skill - orient toward the target and start dishing out attacks.
            if (GetTargetLocation() != null)
            {
                SelfCombatant.GetComponent<OrientationController>().LookAtTarget = GetTargetLocation();
            }
            if (!string.IsNullOrEmpty(SkillAnimationName))
            {
                SelfCombatant.GetComponent<Animator>().SetBool(SkillAnimationName, true);
            }
            var speedMultiplier = SelfCombatant.Attributes.AttackSpeedMultiplier;
            SelfCombatant.GetComponent<Animator>().SetFloat("SkillSpeed", Speed * speedMultiplier);
        }
    }
}
