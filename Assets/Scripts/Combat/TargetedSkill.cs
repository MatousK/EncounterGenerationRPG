using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Represents a skill that can target a single character and has animations.
/// </summary>
public abstract class TargetedSkill : MonoBehaviour
{
    /// <summary>
    /// Number of seconds after which the skill can be used again once it's used.
    /// </summary>
    public float Cooldown;
    /// <summary>
    /// When was this skill used last, in game time.
    /// </summary>
    public float SkillLastUsedAt { get; protected set; } = float.NegativeInfinity;
    /// <summary>
    /// If true, this skill can be used also on characters that are already down, e.g. resurection skills.
    /// </summary>
    public bool CanTargetDownedCharacters;
    /// <summary>
    /// How many spaces away can the character be to start using the skill.
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
    public string SkillAnimationName;
    /// <summary>
    /// If true, this skill is right now being used.
    /// </summary> 
    public bool IsUsingSkill
    {
        get
        {
            return Target != null;
        }
    }
    /// <summary>
    /// Target which we are currently using this skill on.
    /// </summary>
    public CombatantBase Target { get; protected set; }
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
    // Start is called before the first frame update
    protected virtual void Start()
    {
        animationEventListener = GetComponent<AnimationEventsListener>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (Target == null)
        {
            return;
        }
        if (Target.IsDown && !CanTargetDownedCharacters)
        {
            StopSkill();
            return;
        }
        if (!didGetInRange && GetComponent<Collider2D>().Distance(Target.GetComponent<Collider2D>()).distance > Range)
        {
            // Move in range.
            GetComponent<MovementController>().MoveToPosition(Target.transform.position);
            return;
        }
        didGetInRange = true;
        if (!isUsingSkill)
        {
            isUsingSkill = true;
            StartSkillAnimation();
        }
    }
    /// <summary>
    /// Call to start execution of this skill.
    /// </summary>
    /// <param name="target">The target of this skill.</param>
    /// <returns>True if the skill cnan be used right now against the target, otherwise false.</returns>
    public virtual bool UseSkillOn(CombatantBase target)
    {
        if (!CanUseSkill())
        {
            // Already using the skill on someone or we're in cooldown.
            return false;
        }
        didGetInRange = false;
        isUsingSkill = false;
        animationCompletedCount = 0;
        Target = target;
        animationEventListener.ApplySkillEffect += ApplySkillEffects;
        animationEventListener.SkillAnimationFinished += AnimationCompleted;
        return true;
    }
    /// <summary>
    /// Call to stop using this skill.
    /// </summary>
    public void StopSkill()
    {
        if (Target == null)
        {
            // Skill is not being used right now.
            return;
        }
        GetComponent<Animator>().SetBool(SkillAnimationName, false);
        GetComponent<OrientationController>().LookAtTarget = null;
        GetComponent<Animator>().speed = 1;
        Target = null;
        SkillLastUsedAt = Time.time;
        animationEventListener.ApplySkillEffect -= ApplySkillEffects;
        animationEventListener.SkillAnimationFinished -= AnimationCompleted;
    }
    // Return true if this skill can be used at this moment.
    public virtual bool CanUseSkill()
    {
        return this.Target == null && SkillLastUsedAt + Cooldown < Time.time;
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
            StopSkill();
        }
    }

    protected virtual void StartSkillAnimation()
    {
        GetComponent<MovementController>().StopMovement();
        // In range, start using the skill - orient toward the target and start dishing out attacks.
        GetComponent<OrientationController>().LookAtTarget = Target.gameObject;
        GetComponent<Animator>().SetBool(SkillAnimationName, true);
        GetComponent<Animator>().speed = Speed;
    }
}
