using Assets.Scripts.Combat;
using Assets.Scripts.Combat.Skills;
using UnityEngine;

namespace Assets.Scripts.AI.MonsterAI
{
    public class LurkerAi: MonsterAiBase
    {
        public float TeleportSkillMinDistance = 3;
        public TargetedSkill TeleportSkill;

        protected override void Update()
        {
            base.Update();
        }

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void OnActionRequired()
        {
            // Teleport to the current target if too far away, or just start attacking the strongest enemy as much as possible.
            var target = GetCurrentTarget();
            if (target == null)
            {
                return;
            }
            var distanceToTarget = Vector2.Distance(target.transform.position, ControlledCombatant.transform.position);
            if (distanceToTarget > TeleportSkillMinDistance && TryUseSkill(target, TeleportSkill))
            {
                return;
            }
            base.OnActionRequired();
        }

        protected override CombatantBase GetCurrentTarget()
        {
            return ForcedTarget != null ? ForcedTarget : GetStrongestHero();
        }
    }
}