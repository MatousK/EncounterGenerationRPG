using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Camera;
using Assets.Scripts.Combat.Skills;
using Assets.Scripts.Movement;
using UnityEngine;

namespace Assets.Scripts.Combat
{
    public enum AiTargetPriority
    {
        Low,
        Medium,
        High
    }

    public class Hero : CombatantBase
    {
        private CameraMovement cameraMovement;
        private MovementController movementController;
        /// <summary>
        /// The profession this hero has.
        /// </summary>
        public HeroProfession HeroProfession;
        /// <summary>
        /// How much should the character heal after combat if dead.
        /// </summary>
        public float AfterCombatRevivalHealthPercentage = 0.1f;
        /// <summary>
        /// The portrait representing this hero.
        /// </summary>
        public Sprite Portrait;
        /// <summary>
        /// The higher the priority, the more likely is the AI to target this hero.
        /// </summary>
        public AiTargetPriority AiTargetPriority;
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
        /// <summary>
        /// How much should the attack increase when a powerup is picked up.
        /// </summary>
        public float AttackPowerupIncrement;
        /// <summary>
        /// How much should health increase when a powerup is picked up.
        /// </summary>
        public float HealthPowerupIncrement;
        /// <summary>
        /// Will spawn when the order to move to some position is given.
        /// </summary>
        public GameObject MoveToIndicatorTemplate;

        // Start is called before the first frame update
        protected override void Awake()
        {
            base.Awake();
            movementController = GetComponent<MovementController>();
        }

        protected override void Start()
        {
            base.Start();
            CombatantsManager.PlayerCharacters.Add(this);
            cameraMovement = FindObjectOfType<CameraMovement>();
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
            // Auto heal if combat over and dead.
            if (IsDown && !CombatantsManager.IsCombatActive)
            {
                MaxHitpoints += TotalMaxHitpoints * AfterCombatRevivalHealthPercentage;
                GetComponent<Animator>().SetBool("Dead", false);
                GetComponent<Animator>().SetTrigger("Revive");
                // TODO: Figure out how to make reviving animations work.
                // Basically figure out how to not reset animations by calling SetBool.
            }
        }

        public void RevivalDoneOrDeathStarted()
        {
        }

        public virtual void SkillAttackUsed(Monster target)
        {
            // After using a skill, we probably want to keep attacking the enemy.
            GetComponent<AutoAttacking>().Target = target;
            if (EnemyTargetSkill == null || !EnemyTargetSkill.CanUseSkill() || IsBlockingSkillInProgress(false) || IsDown)
            {
                // Special attack either cannot be used or is not defined.
                // Use normal attack started at the start of the method as fallback.
                return;
            }
            target.GetComponent<CommandConfirmationIndicator>().DisplayConfirmation();
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
            if (target != GetComponent<AutoAttacking>().Target)
            {
                // We want the existing attack to be stopped immediately if it is lead on someone else..
                GetComponent<AutoAttacking>().AutoAttackSkill.TryStopSkill();
            }
            GetComponent<AutoAttacking>().Target = target;
            target.GetComponent<CommandConfirmationIndicator>().DisplayConfirmation();
        }

        public virtual void FriendlySkillUsed(Hero target)
        {
            if (IsBlockingSkillInProgress(false) || FriendlyTargetSkill == null || IsDown)
            {
                return;
            }
            target.GetComponent<CommandConfirmationIndicator>().DisplayConfirmation();
            GetComponent<AutoAttacking>().Target = null;
            // Using a skill on a friendly might mean moving towards said friendly.
            // In that case we probably don't want to keep on attacking
            FriendlyTargetSkill.UseSkillOn(target);
        }

        public virtual void FriendlyClicked(Hero target) { }

        public virtual void SelfSkillUsed()
        {
            if (IsBlockingSkillInProgress(false) || SelfTargetSkill == null || IsDown)
            {
                return;
            }
            GetComponent<CommandConfirmationIndicator>().DisplayConfirmation();
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
            foreach (var skill in CombatantSkills.Where(skill => !skill.CanMoveWhileCasting))
            {
                skill.TryStopSkill();
            }
            movementController.MoveToPosition(position);
            var moveToIndicator = Instantiate(MoveToIndicatorTemplate, transform.parent, true);
            moveToIndicator.transform.position = new Vector3(position.x, position.y, -1);

        }
    }
}