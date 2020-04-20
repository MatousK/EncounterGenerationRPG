using Assets.Scripts.Combat;
using Assets.Scripts.Combat.Skills;
using UnityEngine;

namespace Assets.Scripts.AI.MonsterAI
{
    public class MonsterAiBase: AiBase
    {
        /// <summary>
        ///  Once HP of an elite monster drops below this value, it will start using skills. Bosses will use them immediately, normal monsters not at all.
        /// </summary>
        public float EliteSkillHpThreshold = 0.5f;
        /// <summary>
        /// The skill an elite monster should start using if its HP fall to low and which a boss should be using as often as possible.
        /// </summary>
        public Skill AdvancedSkill;

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Update()
        {
            base.Update();
        }

        protected override bool TryDoAction()
        {
            // Try to attack the current target with the advanced skill if possible, with basic attack with that fails.
            // If both of these fails, just do the basic implementation, though something is wrong at this point.
            var monster = (Monster)ControlledCombatant;
            if (monster.Rank == MonsterRank.Boss || (monster.Rank == MonsterRank.Elite && monster.HitPoints / monster.TotalMaxHitpoints < EliteSkillHpThreshold))
            {
                // Use the advanced skill.
                if (TryUseSkill(GetAdvancedSkillTarget(), AdvancedSkill))
                {
                    return true;
                }
            }
            if (TryUseSkill(GetCurrentTarget(), BasicAttack))
            {
                return true;
            }
            return base.TryDoAction();
        }

        /// <summary>
        /// Used by basic implementation of monster AI. Returns the current target which we should be attacking.
        /// </summary>
        /// <returns></returns>
        protected virtual CombatantBase GetCurrentTarget()
        {
            return ForcedTarget != null ? ForcedTarget : GetClosestOpponent();
        }

        protected virtual CombatantBase GetAdvancedSkillTarget()
        {
            return GetCurrentTarget();
        }

        protected CombatantBase GetStrongestHero()
        {
            return GetOpponentWithBestScore(combatant => (int) ((Hero)combatant).AiTargetPriority);
        }
    }
}