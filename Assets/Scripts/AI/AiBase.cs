using System;
using System.Linq;
using Assets.Scripts.Combat;
using Assets.Scripts.Combat.Conditions;
using Assets.Scripts.Combat.Skills;
using Assets.Scripts.Cutscenes;
using UnityEngine;

namespace Assets.Scripts.AI
{
    public class AiBase: MonoBehaviour
    {
        protected CombatantBase ControlledCombatant;
        protected CombatantsManager CombatantsManager;
        protected CutsceneManager CutsceneManager;
        protected TargetedSkill BasicAttack;
        protected bool IsStunned;
        protected CombatantBase ForcedTarget;
        /// <summary>
        /// Used to detect cases when the character cannot reach his target, so he is stuck doing nothing.
        /// </summary>
        protected bool IsProbablyStuck;
        /// <summary>
        /// Last time this character did something.
        /// </summary>
        protected float? LastNonIdleAnimationTime;
        /// <summary>
        /// How long can the character do nothing before we decide he's stuck.
        /// </summary>
        protected const float StuckDetectionInterval = 1;

        protected virtual void Start()
        {
            ControlledCombatant = GetComponentInParent<CombatantBase>();
            CombatantsManager = FindObjectOfType<CombatantsManager>();
            CutsceneManager = FindObjectOfType<CutsceneManager>();
            BasicAttack = ControlledCombatant.CombatantSkills.FirstOrDefault(skill => skill.IsBasicAttack) as TargetedSkill;
        }
        protected virtual void Update()
        {
            if (ControlledCombatant.IsDown)
            {
                return;
            }
            UpdateIsStunnedAndForcedTarget();
            if (!ControlledCombatant.IsBlockingSkillInProgress(true) && !CutsceneManager.IsCutsceneActive && !IsStunned)
            {
                TryDoAction();
            }
            // Really hacky way to unstuck the character if he cannot reach the target.
            var characterAnimator = ControlledCombatant.GetComponent<Animator>();
            bool isIdle = !characterAnimator.GetBool("Walking") && !characterAnimator.GetBool("Attacking") && !characterAnimator.GetBool("Gesturing") && !characterAnimator.GetBool("Dead") && !characterAnimator.GetBool("Asleep");
            if (!isIdle)
            {
                LastNonIdleAnimationTime = Time.time;
                IsProbablyStuck = false;
            }
            if (LastNonIdleAnimationTime != null && Time.time - LastNonIdleAnimationTime.Value > StuckDetectionInterval && !IsProbablyStuck)
            {
                IsProbablyStuck = true;
                foreach (var skill in ControlledCombatant.CombatantSkills)
                {
                    skill.TryStopSkill();
                }
            }
        }

        protected virtual bool TryDoAction()
        {
            var target = ForcedTarget != null ? ForcedTarget : GetClosestOpponent();
            return TryUseSkill(target, BasicAttack);
        }

        protected bool TryUseSkill(CombatantBase target, Skill skill)
        {
            if (skill is PersonalSkill && skill != null && skill.CanUseSkill())
            {
                ((PersonalSkill)skill).ActivateSkill();
            }
            if (!(skill is TargetedSkill) || target == null || skill == null || !skill.CanUseSkill())
            {
                return false;
            }
            ((TargetedSkill)skill).UseSkillOn(target);
            return true;
        }

        protected CombatantBase GetClosestOpponent()
        {
            return GetOpponentWithBestScore(opponent => -Vector2.Distance(opponent.transform.position, ControlledCombatant.transform.position));
        }

        protected CombatantBase GetOpponentWithBestScore(Func<CombatantBase, float> scoreFunction)
        {
            CombatantBase toReturn = null;
            float currentBestScore = float.NegativeInfinity;
            var opponents = CombatantsManager.GetOpponentsFor(ControlledCombatant, onlyAlive: true);
            foreach (var combatant in opponents)
            {
                var score = scoreFunction(combatant);
                if (score > currentBestScore)
                {
                    currentBestScore = score;
                    toReturn = combatant;
                }
            }
            return toReturn;
        }

        protected void UpdateIsStunnedAndForcedTarget()
        {
            IsStunned = false;
            ForcedTarget = null;
            foreach (var condition in ControlledCombatant.GetComponent<ConditionManager>().ActiveConditions)
            {
                if (condition is StunCondition)
                {
                    IsStunned = true;
                }
                else if (condition is ForcedTargetCondition forceTargetCondition)
                {
                    ForcedTarget = forceTargetCondition.ForcedTarget;
                }
            }
            ForcedTarget = ForcedTarget != null && !ForcedTarget.IsDown ? ForcedTarget : null;
        }
    }
}