using System.Linq;
using Assets.Scripts.Combat;
using Assets.Scripts.Combat.Skills;
using Assets.Scripts.Movement;
using UnityEngine;

namespace Assets.Scripts.AI.MonsterAI
{
    public class LeaderAi : MonsterAiBase
    {
        public float HealingAuraMoveToRange = 2;
        public TargetedSkill TargetHeroSkill;

        protected override void Awake()
        {
            base.Awake();
        }
        protected override void Update()
        {
            base.Update();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        protected override void OnActionRequired()
        {
            var target = GetCurrentTarget() as Hero;
            if (target == null)
            {
                // Heroes are dead.
            }
            var alliedMonsters = CombatantsManager.GetOpponentsFor(ControlledCombatant, onlyAlive: true).ToArray();
            // Targeting is unnecessary when fighting alongside a low amount of monsters.
            if (target.AiTargetPriority == AiTargetPriority.High && alliedMonsters.Length >= 3)
            {
                // High priority target is alive, target him and kill him quickly.
                if (TryUseSkill(target, TargetHeroSkill))
                {
                    return;
                }
            }
            if (AdvancedSkill.IsBeingUsed())
            {
                // The advanced skill is for healing. Stay close to the most wounded ally.
                CombatantBase mostWoundedAlly = null;
                float mostWoundedAllyHpPercentage = 1;
                foreach (var ally in alliedMonsters)
                {
                    var allyHpPercentage = ally.HitPoints / ally.MaxHitpoints;
                    if (allyHpPercentage < mostWoundedAllyHpPercentage && ally != ControlledCombatant)
                    {
                        mostWoundedAllyHpPercentage = allyHpPercentage;
                        mostWoundedAlly = ally;
                    }
                }
                if (mostWoundedAlly != null)
                {
                    if (Vector2.Distance(mostWoundedAlly.transform.position, ControlledCombatant.transform.position) > HealingAuraMoveToRange)
                    {
                        ControlledCombatant.GetComponent<MovementController>().MoveToPosition(mostWoundedAlly.transform.position);
                        return;
                    }
                }
            }
            base.OnActionRequired();
        }

        protected override CombatantBase GetCurrentTarget()
        {
            return ForcedTarget != null ? ForcedTarget : GetStrongestHero();
        }

        protected override CombatantBase GetAdvancedSkillTarget()
        {
            return ControlledCombatant;
        }
    }
}
