using Assets.Scripts.Combat;
using Assets.Scripts.Combat.Skills;
using UnityEngine;

namespace Assets.Scripts.AI.MonsterAI
{
    /// <summary>
    /// An AI for lurkers. Unlike regular monsters, the lurker selects the most dangerous hero as his target.
    /// If the teleport skill is set (which should be done only for boss), it will also try to teleport to the target if possible.
    /// </summary>
    public class LurkerAi: MonsterAiBase
    {
        /// <summary>
        /// If the target is further than this, the Lurker will try to teleport to him if possible.
        /// </summary>
        public float TeleportSkillMinDistance = 3;
        /// <summary>
        /// The skill used for teleporting. If null, the lurker will not teleport.
        /// </summary>
        public TargetedSkill TeleportSkill;

        protected override void Update()
        {
            base.Update();
        }

        protected override void Start()
        {
            base.Start();
        }
        /// <summary>
        /// Teleport to the current target if too far away and teleporting is possible.
        /// Otherwise just do standard monster AI, which is attacking the target.
        /// </summary>
        /// <returns>True if some action was executed, otherwise false.</returns>
        protected override bool TryDoAction()
        {
            var target = GetCurrentTarget();
            if (target == null)
            {
                return false;
            }
            var distanceToTarget = Vector2.Distance(target.transform.position, ControlledCombatant.transform.position);
            if (distanceToTarget > TeleportSkillMinDistance && TryUseSkill(target, TeleportSkill))
            {
                return true;
            }
            return base.TryDoAction();
        }
        /// <summary>
        /// The target for the lurker is either the forced target or the strongest hero.
        /// </summary>
        /// <returns>The target of the attack, or null if noone should be attacked.</returns>
        protected override CombatantBase GetCurrentTarget()
        {
            return ForcedTarget != null ? ForcedTarget : GetStrongestHero();
        }
    }
}