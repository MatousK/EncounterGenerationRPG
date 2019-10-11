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
    /// Target which we are currently using this skill on.
    /// </summary>
    public CombatantBase Target { get; protected set; }
    /// <summary>
    /// If true, the skill can target characters who are already dead.
    /// </summary>
    [NonSerialized]
    public bool CanTargetDownedCharacters;
    protected override void Start()
    {
        base.Start();
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
        return GetComponent<Collider2D>().Distance(Target.GetComponent<Collider2D>()).distance;
    }

    public override Vector2 GetTargetLocation()
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
}
