using System;
using Assets.Scripts.Combat.Conditions;
using UnityEngine;

namespace Assets.Scripts.Combat.Skills.Character.Ranger
{
    class PoisonCloud : TargetedGestureSkill
    {
        public float PoisonCloudRange = 3;
        public float PoisonDuration = 10;
        public GameObject PoisonCloudEffect = null;
        private CombatantsManager combatantsManager;

        protected override void Awake()
        {
            base.Awake();
            combatantsManager = FindObjectOfType<CombatantsManager>();
        }

        protected override void Update()
        {
            base.Update();
        }

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