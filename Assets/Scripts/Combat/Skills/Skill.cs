using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
/// <summary>
/// Represents a skill a character can use.
/// </summary>
public abstract class Skill: MonoBehaviour
{
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
    public int Range = 1;
    /// <summary>
    /// How many times does the animation repeat as part of one skill usage.
    /// </summary>
    public int Repetitions = 1;
    /// <summary>
    /// How fast should the animation play. 1 is start, 2 is twice as fast, 0.5 is half as slow etc.
    /// </summary>
    public float Speed = 1;
    /// <summary>
    /// The name of the animation that should be played while this skill is being used.
    /// </summary>
    [NonSerialized]
    public string SkillAnimationName;
    /// <summary>
    /// This should fire events when a skill animation reaches points when we should react to it.
    /// </summary>
    protected AnimationEventsListener animationEventListener;
    /// <summary>
    /// If true, this character already moved in range and is free to continue using the skill regardless of range.
    /// </summary>
    protected bool didGetInRange;
    /// <summary>
    /// If true, this skill is being used right now.
    /// </summary>
    protected bool isUsingSkill;
    /// <summary>
    /// How many times was this animation already completed while using this skill
    /// </summary>
    protected int animationCompletedCount;
    /// <summary>
    /// Combatant who is using this skill.
    /// </summary>
    protected CombatantBase selfCombatant;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        animationEventListener = GetComponent<AnimationEventsListener>();
        selfCombatant = GetComponent<CombatantBase>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (!IsBeingUsed())
        {
            return;
        }
        var rangeMultiplier = selfCombatant?.Attributes?.RangeMultiplier ?? 1;
        if (!didGetInRange && GetDistanceToTargetLocation() >= Range * rangeMultiplier)
        {
            // Move in range.
            GetComponent<MovementController>().MoveToPosition(GetTargetLocation());
            return;
        }
        didGetInRange = true;
        if (!isUsingSkill)
        {
            isUsingSkill = true;
            StartSkillAnimation();
        }
    }
    public abstract float GetDistanceToTargetLocation();
    public abstract Vector2 GetTargetLocation();
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
            selfCombatant.StartCooldown(Cooldown);
        }
        didGetInRange = false;
        isUsingSkill = false;
        animationCompletedCount = 0;
        animationEventListener.ApplySkillEffect += ApplySkillEffects;
        animationEventListener.SkillAnimationFinished += AnimationCompleted;
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
        GetComponent<Animator>().SetBool(SkillAnimationName, false);
        GetComponent<OrientationController>().LookAtTarget = null;
        animationEventListener.ApplySkillEffect -= ApplySkillEffects;
        animationEventListener.SkillAnimationFinished -= AnimationCompleted;
        return true;
    }
    // Return true if this skill can be used at this moment.
    public virtual bool CanUseSkill()
    {
        return !IsBeingUsed() && (selfCombatant.LastSkillRemainingCooldown ?? 0) <= 0;
    }
    /// <summary>
    /// Called when the skill animation hits the point where the effects should be applied
    /// </summary>
    protected abstract void ApplySkillEffects(object sender, EventArgs e);
    /// <summary>
    /// Called when the skill animation completes.
    /// Default implementation will stop using the skill if this method was called sufficient amount of times, <see cref="Repetitions"/>
    /// </summary>
    protected virtual void AnimationCompleted(object sender, EventArgs e)
    {
        animationCompletedCount++;
        if (animationCompletedCount >= Repetitions)
        {
            TryStopSkill();
        }
    }

    protected virtual void StartSkillAnimation()
    {
        GetComponent<MovementController>().StopMovement();
        // In range, start using the skill - orient toward the target and start dishing out attacks.
        GetComponent<OrientationController>().LookAtTarget = GetTargetLocation();
        GetComponent<Animator>().SetBool(SkillAnimationName, true);
        var speedMultiplier = selfCombatant?.Attributes?.AttackSpeedMultiplier ?? 1;
        GetComponent<Animator>().SetFloat("SkillSpeed", Speed * speedMultiplier);
    }
}
