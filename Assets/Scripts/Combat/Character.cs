using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Character : CombatantBase
{
    private MovementController movementController;
    /// <summary>
    /// A skill that should be used when using a skill on self.
    /// </summary>
    public PersonalSkill SelfTargetSkill;
    /// <summary>
    /// A skill that should be used when using a skill on an enemy.
    /// </summary>
    public TargetedSkill EnemyTargetSkill;
    /// <summary>
    /// A skill that should be used when using a skill on self.
    /// </summary>
    public TargetedSkill FriendlyTargetSkill;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        FindObjectOfType<CombatantsManager>().PlayerCharacters.Add(this);
        movementController = GetComponent<MovementController>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public virtual void SkillAttackUsed(Enemy target)
    {
        // After using a skill, we probably want to keep attacking the enemy.
        GetComponent<AutoAttacking>().Target = target;
        if (EnemyTargetSkill?.CanUseSkill() != true || IsSkillUsageBlocked())
        {
            // Special attack either cannot be used or is not defined.
            // Use normal attack started at the start of the method as fallback.
            return;
        }
        // We do not want multiple skills being executed simoultaneously.
        GetComponent<AutoAttacking>()?.AutoAttackSkill?.TryStopSkill();
        EnemyTargetSkill?.UseSkillOn(target);
    }

    public virtual void AttackUsed(Enemy target)
    {
        GetComponent<AutoAttacking>().Target = target;
    }

    public virtual void FriendlySkillUsed(Character target)
    {
        if (IsSkillUsageBlocked())
        {
            return;
        }
        // Using a skill on a friendly might mean moving towards said friendly.
        // In that case we probably don't want to keep on attacking
        FriendlyTargetSkill?.UseSkillOn(target);
        GetComponent<AutoAttacking>().Target = null;
    }

    public virtual void FriendlyClicked(Character target) { }

    public virtual void SelfSkillUsed()
    {
        if (IsSkillUsageBlocked())
        {
            return;
        }
        SelfTargetSkill?.ActivateSkill();
    }

    public virtual void SelfClicked(){ }

    public virtual void LocationSkillClick(Vector2 position)
    {
        if (IsManualMovementBlocked())
        {
            return;
        }
        GetComponent<AutoAttacking>().Target = null;
        movementController.MoveToPosition(position);
    }

    public virtual void LocationClick(Vector2 position)
    {
        if (IsManualMovementBlocked())
        {
            return;
        }
        GetComponent<AutoAttacking>().Target = null;
        movementController.MoveToPosition(position);
    }
}
