using Assets.Scripts.Combat;
using Assets.Scripts.Combat.Skills;
using UnityEngine;

namespace Assets.Scripts.AI.MonsterAI
{
    /// <summary>
    /// AI for monsters. Unlike the hero AI, this is actually used in the game.
    /// Default behavior will attack its target, which is either the target forced by the knight or by leader or the closest enemy.
    /// Child classes can override who is the target for this monster.
    /// Monsters can also use skills. Regular enemies never use them, elite enemies use them when their health drops below 50% and bosses use them whenever possible.
    /// </summary>
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
        /// If it should use a skill (it is a boss or hurt elite), it will do so.
        /// Otherwise it will just start attacking its target, see <see cref="GetCurrentTarget"/>
        /// </summary>
        /// <returns>Returns true if some action was executed, otherwise false.</returns>
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
        /// Returns the current target which we should be attacking.
        /// By default it will be the forced target. If no target is set, target the closest enemy.
        /// </summary>
        /// <returns> The target for this monster, or null if this monster is not targeting anyone.</returns>
        protected virtual CombatantBase GetCurrentTarget()
        {
            return ForcedTarget != null ? ForcedTarget : GetClosestOpponent();
        }

        /// <summary>
        /// Returns the target for the current skill.
        /// Normally it will be the current target, but it might be someone else in case of self or friendly skills.
        /// </summary>
        /// <returns></returns>
        protected virtual CombatantBase GetAdvancedSkillTarget()
        {
            return GetCurrentTarget();
        }
        /// <summary>
        /// Retrieve the most powerful hero alive.
        /// </summary>
        /// <returns>The strongest hero who is alive, or null if no heroes are alive.</returns>
        protected CombatantBase GetStrongestHero()
        {
            return GetOpponentWithBestScore(combatant => (int) ((Hero)combatant).AiTargetPriority);
        }
    }
}