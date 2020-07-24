using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Camera;
using Assets.Scripts.Combat.Skills;
using Assets.Scripts.Movement;
using UnityEngine;

namespace Assets.Scripts.Combat
{
    /// <summary>
    /// Some AIs use values from this enum to determine which enemies to target first.
    /// Some AIs ignore this.
    /// </summary>
    public enum AiTargetPriority
    {
        Low,
        Medium,
        High
    }

    public class Hero : CombatantBase
    {
        /// <summary>
        /// Component used to move the hero around.
        /// </summary>
        private MovementController movementController;
        /// <summary>
        /// The profession this hero has.
        /// </summary>
        public HeroProfession HeroProfession;
        /// <summary>
        /// When a character is revived after combat, he will be automatically healed enough to have this many percent of hit points.
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
        /// A skill that should be used when using a skill on an ally.
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
            }
        }
        /// <summary>
        /// Orders the player to use the <see cref="EnemyTargetSkill"/> against some monster.
        /// If <see cref="Skill.ClearTargetAfterUsingSkill"/> if true, this will clear auto attacking target.
        /// If it is false, the hero will then keep attacking the taget.
        /// </summary>
        /// <param name="target">The target of this skill.</param>
        public virtual void SkillAttackUsed(Monster target)
        {
            // After using a skill, we probably want to keep attacking the enemy.
            GetComponent<AutoAttacking>().Target = target;
            if (EnemyTargetSkill == null || !EnemyTargetSkill.CanUseSkill() || IsBlockingSkillInProgress(false) || IsDown)
            {
                // Special attack either cannot be used or is not defined.
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
        /// <summary>
        /// Start attacking the target monster with basic attack.
        /// </summary>
        /// <param name="target">Target of this attack</param>
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
        /// <summary>
        /// Use a friendly skill on the specified ally.
        /// Will clear auto attacking.
        /// </summary>
        /// <param name="target">Target of this skill.</param>
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
        /// <summary>
        /// Called when the player right clicks on a hero without using a skill. Does nothing by default.
        /// </summary>
        /// <param name="target">Ally that was clicked.</param>
        public virtual void FriendlyClicked(Hero target) { }
        /// <summary>
        /// Use the self skill on the hero himself.
        /// If <see cref="Skill.ClearTargetAfterUsingSkill"/>, this will also clear auto attacking target.
        /// </summary>
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
        /// <summary>
        /// Called when is right clicked on himself when not using a skill. Does nothing by default.
        /// </summary>
        public virtual void SelfClicked(){ }
        /// <summary>
        /// Right clicking on a location while using a skill. The hero will start movement to that location.
        /// The skill is ignored.
        /// </summary>
        /// <param name="position">The position where the player clicked.</param>
        public virtual void LocationSkillClick(Vector2 position)
        {
            MoveToCommand(position);
        }
        /// <summary>
        /// Right clicking on a location without using a skill. The hero will start movement to that location.
        /// </summary>
        /// <param name="position">The position where the player clicked.</param>
        public virtual void LocationClick(Vector2 position)
        {
            MoveToCommand(position);
        }
        /// <summary>
        /// Gives the hero an order to move to the specified position.
        /// </summary>
        /// <param name="position">Position where the hero should move.</param>
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