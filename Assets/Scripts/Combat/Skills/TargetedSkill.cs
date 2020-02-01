using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Represents a skill that can target a single character and has animations.
/// </summary>
public abstract class TargetedSkill : Skill
{
    /// <summary>
    /// How many times does the animation repeat as part of one skill usage.
    /// </summary>
    public int Repetitions = 1;
    /// <summary>
    /// Target which we are currently using this skill on.
    /// </summary>
    public CombatantBase Target { get; protected set; }
    /// <summary>
    /// If true, the skill can target characters who are already dead.
    /// </summary>
    [NonSerialized]
    public bool CanTargetDownedCharacters;
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Update()
    {
        if (Target && Target.IsDown && !CanTargetDownedCharacters)
        {
            TryStopSkill();
            return;
        }
        base.Update();
    }

    public override float GetDistanceToTargetLocation()
    {
        return selfCombatant.GetComponent<Collider2D>().Distance(Target.GetComponent<Collider2D>()).distance;
    }

    public override Vector2? GetTargetLocation()
    {
        return Target.transform.position;
    }

    public override bool IsBeingUsed()
    {
        return Target != null;
    }

    public bool UseSkillOn(CombatantBase target)
    {
        var toReturn = TryStartUsingSkill();
        if (toReturn)
        {
            Target = target;
        }
        return toReturn;
    }

    public override bool TryStopSkill()
    {
        var didStopSkill = base.TryStopSkill();
        if (didStopSkill)
        {
            Target = null;
        }
        return didStopSkill;
    }
    /// <summary>
    /// Called when the skill animation completes.
    /// Default implementation will stop using the skill if this method was called sufficient amount of times, <see cref="Repetitions"/>
    /// </summary>
    protected override void AnimationCompleted(object sender, EventArgs e)
    {
        animationCompletedCount++;
        if (animationCompletedCount >= Repetitions)
        {
            TryStopSkill();
        }
    }
}
