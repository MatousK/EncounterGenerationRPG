using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Hero : CombatantBase
{
    private CameraMovement cameraMovement;
    private MovementController movementController;
    /// <summary>
    /// The higher the priority, the more likely is the AI to target this hero.
    /// </summary>
    public int AITargetPriority;
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
    protected override void Awake()
    {
        base.Awake();
        FindObjectOfType<CombatantsManager>().PlayerCharacters.Add(this);
        movementController = GetComponent<MovementController>();
        cameraMovement = FindObjectOfType<CameraMovement>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public virtual void SkillAttackUsed(Monster target)
    {
        // After using a skill, we probably want to keep attacking the enemy.
        GetComponent<AutoAttacking>().Target = target;
        if (EnemyTargetSkill == null || !EnemyTargetSkill.CanUseSkill() || IsBlockingSkillInProgress(false))
        {
            // Special attack either cannot be used or is not defined.
            // Use normal attack started at the start of the method as fallback.
            return;
        }
        // We do not want multiple skills being executed simoultaneously.
        GetComponent<AutoAttacking>().AutoAttackSkill.TryStopSkill();
        EnemyTargetSkill.UseSkillOn(target);
        if (EnemyTargetSkill.ClearTargetAfterUsingSkill)
        {
            GetComponent<AutoAttacking>().Target = null;
        }
    }

    public virtual void AttackUsed(Monster target)
    {
        GetComponent<AutoAttacking>().Target = target;
    }

    public virtual void FriendlySkillUsed(Hero target)
    {
        if (IsBlockingSkillInProgress(false) || FriendlyTargetSkill == null)
        {
            return;
        }
        GetComponent<AutoAttacking>().Target = null;
        // Using a skill on a friendly might mean moving towards said friendly.
        // In that case we probably don't want to keep on attacking
        FriendlyTargetSkill.UseSkillOn(target);
    }

    public virtual void FriendlyClicked(Hero target) { }

    public virtual void SelfSkillUsed()
    {
        if (IsBlockingSkillInProgress(false) || SelfTargetSkill == null)
        {
            return;
        }
        SelfTargetSkill.ActivateSkill();
        if (SelfTargetSkill.ClearTargetAfterUsingSkill)
        {
            GetComponent<AutoAttacking>().Target = null;
        }
    }

    public virtual void SelfClicked(){ }

    public virtual void LocationSkillClick(Vector2 position)
    {
        MoveToCommand(position);
    }

    public virtual void LocationClick(Vector2 position)
    {
        MoveToCommand(position);
    }

    protected void MoveToCommand(Vector2 position)
    {
        if (IsManualMovementBlocked())
        {
            return;
        }
        GetComponent<AutoAttacking>().Target = null;
        if (!combatantsManager.IsCombatActive)
        {
            cameraMovement.FollowingHero = this;
            movementController.MoveToPosition(position, onMoveToSuccessful: (success) => cameraMovement.FollowingHero = null);
        }
        else
        {
            movementController.MoveToPosition(position);
        }
    }
}
