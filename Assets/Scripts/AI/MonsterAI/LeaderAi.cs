using System.Linq;
using Assets.Scripts.Combat;
using Assets.Scripts.Combat.Skills;
using Assets.Scripts.Movement;
using UnityEngine;

namespace Assets.Scripts.AI.MonsterAI
{
    /// <summary>
    /// AI for leaders. Leader will always try to force its allies to kill the ranger if possible.
    /// If a ranger is not active, the monster will use the standard monster behavior until a skill is triggered.
    /// While the healing aura is active, the leader will try to move close to hurt allies.
    /// </summary>
    public class LeaderAi : MonsterAiBase
    {
        /// <summary>
        /// How close should the cleric move to hurt allies.
        /// </summary>
        public float HealingAuraMoveToRange = 2;
        /// <summary>
        /// The skill the Leader should use when he wants to target his enemies.
        /// </summary>
        public TargetedSkill TargetHeroSkill;

        protected override void Start()
        {
            base.Start();
        }
        protected override void Update()
        {
            base.Update();
        }
        /// <summary>
        /// Called whenever the AI should do something.
        /// It will target the ranger if possible and if enough allies are still alive.
        /// Otherwise if the base class activated the healing aura, the cleric will try to stay close to damaged allies.
        /// Otherwise will execute default monster AI.
        /// </summary>
        /// <returns>True if some action was executed, otherwise false.</returns>
        protected override bool TryDoAction()
        {
            var target = GetCurrentTarget() as Hero;
            if (target == null)
            {
                // Heroes are dead.
                return false;
            }
            var alliedMonsters = CombatantsManager.GetOpponentsFor(ControlledCombatant, onlyAlive: true).ToArray();
            // Targeting is unnecessary when fighting alongside a low amount of monsters or if the ranger is not the current target.
            if (target.AiTargetPriority == AiTargetPriority.High && alliedMonsters.Length >= 3)
            {
                // High priority target is alive, target him and kill him quickly.
                if (TryUseSkill(target, TargetHeroSkill))
                {
                    return true;
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
                        return true;
                    }
                }
            }
            return base.TryDoAction();
        }
        /// <summary>
        /// Return the target for the leader, which is either the forced target or the strongest hero.
        /// </summary>
        /// <returns> The target the leader should attack. </returns>
        protected override CombatantBase GetCurrentTarget()
        {
            return ForcedTarget != null ? ForcedTarget : GetStrongestHero();
        }
        /// <summary>
        /// Return the target of the Healing Aura ability, which is always the cleric himself.
        /// </summary>
        /// <returns>The cleric's combatant component.</returns>
        protected override CombatantBase GetAdvancedSkillTarget()
        {
            return ControlledCombatant;
        }
    }
}
