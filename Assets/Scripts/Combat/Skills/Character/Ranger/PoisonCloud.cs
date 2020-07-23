using System;
using Assets.Scripts.Combat.Conditions;
using UnityEngine;

namespace Assets.Scripts.Combat.Skills.Character.Ranger
{
    /// <summary>
    /// Creates a poison cloud around an ally which gives the poisoned condition to the opponents around that ally.
    /// <see cref="PoisonedCondition"/>
    /// </summary>
    class PoisonCloud : TargetedGestureSkill
    {
        /// <summary>
        /// How far around the ally should the poison cloud work.
        /// </summary>
        public float PoisonCloudRange = 3;
        /// <summary>
        /// How long should the affected opponents remain poisoned.
        /// </summary>
        public float PoisonDuration = 10;
        /// <summary>
        /// Object that appears around the ally to indicate that the poison cloud was cast.
        /// </summary>
        public GameObject PoisonCloudEffect = null;
        /// <summary>
        /// The class which knows about all the combatants in the game.
        /// </summary>
        private CombatantsManager combatantsManager;

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
        /// <inheritdoc/> Gives the poisoned condition to all opponents around that ally.
        /// <see cref="PoisonedCondition"/>
        /// <see cref="PoisonDuration"/>
        /// </summary>
        /// <param name="sender">The sender of this event.</param>
        /// <param name="e">Arguments of this event.</param>
        protected override void ApplySkillEffects(object sender, EventArgs e)
        {
            base.ApplySkillEffects(sender, e);
            var poisonCloudEffect = Instantiate(PoisonCloudEffect, Target.transform, false);
            poisonCloudEffect.SetActive(true);
            foreach (var enemy in combatantsManager.GetEnemies(onlyAlive:true))
            {
                if (Vector2.Distance(enemy.transform.position, Target.transform.position) < PoisonCloudRange)
                {
                    var poisonCondition = enemy.GetComponent<ConditionManager>().AddCondition<PoisonedCondition>();
                    poisonCondition.RemainingDuration = PoisonDuration;
                }
            }
        }
    }
}