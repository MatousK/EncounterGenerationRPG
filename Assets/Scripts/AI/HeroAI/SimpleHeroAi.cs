using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Combat.Skills;
using Assets.Scripts.Extension;

namespace Assets.Scripts.AI.HeroAI
{
    /// <summary>
    /// As our AIs proved to be a lot better than a player playing for the first time, we prepared an AI that is much simpler.
    /// It will attack the closest enemies and from time to time use a skill, not really thinking about how appropriate it is.
    /// This is only meant to be used in the combat simulator.
    /// </summary>
    class SimpleHeroAi : HeroAiBase
    {
        /// <summary>
        /// Every time the hero should do an action, this is a probability he will use a skill.
        /// </summary>
        private const float SkillUsageProbability = 0.2f;
        /// <summary>
        /// Probability that the hero will try to target an ally with his skills.
        /// </summary>
        private const float FriendlyTargetProbability = 0.5f;
        /// <summary>
        /// Called when an action is requested from the hero.
        /// It will randomly either use a skill on a random target or attack the closest opponent
        /// </summary>
        /// <returns>True if some action was done, otherwise false. </returns>
        protected override bool TryDoAction()
        {
            if (UnityEngine.Random.Range(0f, 1f) < SkillUsageProbability)
            {
                if (UnityEngine.Random.Range(0f, 1f) < FriendlyTargetProbability)
                {
                    var allies = CombatantsManager.GetAlliesFor(ControlledCombatant, onlyAlive: true);
                    var target = allies.GetRandomElementOrDefault();
                    var usedSkill = target == ControlledCombatant
                        ? ControlledHero.SelfTargetSkill
                        : (Skill)ControlledHero.FriendlyTargetSkill;
                    if (TryUseSkill(target, usedSkill))
                    {
                        return true;
                    }
                    // Skill could not be use, try to use an attack skill or just do basic attack.;
                }
                // Friendly skill either failed or was not selected, try to do enemy target skill.
                if (TryUseSkill(GetClosestOpponent(), ControlledHero.EnemyTargetSkill))
                {
                    return true;
                }
            }
            // Skill either cannot be used or was not supposed to be used.
            var basicAttackTarget = GetClosestOpponent();
            return TryUseSkill(basicAttackTarget, BasicAttack);
        }
    }
}
