using System;
using System.Linq;
using Assets.Scripts.Combat;
using Assets.Scripts.Combat.Conditions;
using Assets.Scripts.Combat.Skills;
using Assets.Scripts.Cutscenes;
using UnityEngine;

namespace Assets.Scripts.AI
{
    /// <summary>
    /// Children of this class can attached to combatant game objects to give them some artificial intelligence.
    /// If not methods overriden, this AI will just attack the closest opponent. 
    /// </summary>
    public class AiBase: MonoBehaviour
    {
        /// <summary>
        /// The combatant this AI controls.
        /// </summary>
        protected CombatantBase ControlledCombatant;
        /// <summary>
        /// Component that has info about all present combatants.
        /// </summary>
        protected CombatantsManager CombatantsManager;
        /// <summary>
        /// Component that knows whether we are in a cutscene or not, used to block AI during cutscenes.
        /// </summary>
        protected CutsceneManager CutsceneManager;
        /// <summary>
        /// Skill to be used as a basic attack.
        /// </summary>
        protected TargetedSkill BasicAttack;
        /// <summary>
        /// If true, this combatant is stunned and should not do anything.
        /// </summary>
        protected bool IsStunned;
        /// <summary>
        /// If not null, this AI has a forced target for its abilites set by an enemy cleric or the knight.
        /// </summary>
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
        /// <summary>
        /// Fill references to necessary components in the scene.
        /// </summary>
        protected virtual void Start()
        {
            ControlledCombatant = GetComponentInParent<CombatantBase>();
            CombatantsManager = FindObjectOfType<CombatantsManager>();
            CutsceneManager = FindObjectOfType<CutsceneManager>();
            BasicAttack = ControlledCombatant.CombatantSkills.FirstOrDefault(skill => skill.IsBasicAttack) as TargetedSkill;
        }
        /// <summary>
        /// Every frame this component updates internal IsStuck and ForcedTarget flags and tries to do an action if possible.
        /// It will also detect whether the AI is stuck doing nothing.
        /// </summary>
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
                // Stuck! Set the flag so the skills start targeting closest enemies and stop all skills in progress.
                IsProbablyStuck = true;
                foreach (var skill in ControlledCombatant.CombatantSkills)
                {
                    skill.TryStopSkill();
                }
            }
        }
        /// <summary>
        /// Called whenever the AI should do something. Expected to be overriden by child classes.
        /// </summary>
        /// <returns>True if some action was executed, otherwise false.</returns>
        protected virtual bool TryDoAction()
        {
            var target = ForcedTarget != null ? ForcedTarget : GetClosestOpponent();
            return TryUseSkill(target, BasicAttack);
        }
        /// <summary>
        /// Attempts to use the specified skill on the specified target.
        /// </summary>
        /// <param name="target">The target of the skill.</param>
        /// <param name="skill">The skill to be used.</param>
        /// <returns>True if the skill could be used.</returns>
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
        /// <summary>
        /// We retrieve the closest opponent for this AI.
        /// </summary>
        /// <returns>The closest opponent, or null if all opponents are dead.</returns>
        protected CombatantBase GetClosestOpponent()
        {
            // Using negative distance, as the function used returns only the highest score and we need the one with the lowest score.
            return GetOpponentWithBestScore(opponent => -Vector2.Distance(opponent.transform.position, ControlledCombatant.transform.position));
        }
        /// <summary>
        /// Finds all opponents. For each of these opponents calculates a score and returns the opponent with the highest score
        /// </summary>
        /// <param name="scoreFunction">The function that assigns some score to the opponent.</param>
        /// <returns> The opponent with the best score, or null if no opponents are present for this AI. </returns>
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
        /// <summary>
        /// Updates the flags telling the AI whether the combatant is stunned or currently forced to attack someone specific.
        /// </summary>
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