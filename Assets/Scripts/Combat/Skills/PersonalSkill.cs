using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
/// <summary>
/// Represents a skill that affects only the character casting it.
/// </summary>
public abstract class PersonalSkill : Skill
{
    public PersonalSkill()
    {
        // Personal skills start and stop on timer.
        Repetitions = int.MaxValue;
        // Usually a personal skill does no
        BlocksOtherSkills = false;
        BlocksManualMovement = false;
    }
    [NonSerialized]
    public float Duration = 1;
    /// <summary>
    /// If true, the skill is active right now
    /// </summary>
    protected bool IsActive;
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
    /// Gets the target location of this skill. For personal skills, this is always the combatant's position.
    /// </summary>
    /// <returns>Target of the skill, i.e. combatant's position.</returns>
    public override Vector2 GetTargetLocation()
    {
        return transform.position;
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
