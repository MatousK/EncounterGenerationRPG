using System;
using Assets.Scripts.Combat.Conditions;
using Assets.Scripts.Effects;

namespace Assets.Scripts.Combat.Skills.Monster.Leader
{
    /// <summary>
    /// Orders all other monsters to attack the target.
    /// </summary>
    public class CallTarget : TargetedGestureSkill
    {
        /// <summary>
        /// The class which knows about all existing combatants.
        /// </summary>
        CombatantsManager combatantsManager;
        /// <summary>
        /// Determines how long will the target call last.
        /// </summary>
        public float Duration;

        protected override void Start()
        {
            base.Start();
            combatantsManager = FindObjectOfType<CombatantsManager>();
        }
        protected override void Update()
        {
            base.Update();
        }
        /// <summary>
        /// Apply this skill, which means give all allies the forced target condition on the target.
        /// <see cref="ForcedTargetCondition"/>
        /// </summary>
        /// <param name="sender">The sender of this event.</param>
        /// <param name="e">Arguments of this event.</param>
        protected override void ApplySkillEffects(object sender, EventArgs e)
        {
            foreach (var alliedMonster in combatantsManager.GetEnemies(onlyAlive: true))
            {
                if (alliedMonster == SelfCombatant)
                {
                    // The leader does not take orders from himself.
                    continue;
                }
                var conditionManager = alliedMonster.GetComponent<ConditionManager>();
                var addedCondition = conditionManager.AddCondition<ForcedTargetCondition>();
                addedCondition.ForcedTarget = Target;
                addedCondition.RemainingDuration = Duration;
                addedCondition.TargetForcedBy = SelfCombatant;
            }
            SelfCombatant.GetComponentInChildren<TauntEffect>().StartEffect();
            base.ApplySkillEffects(sender, e);
        }
    }
}