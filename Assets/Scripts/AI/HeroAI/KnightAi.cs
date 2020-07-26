using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Combat;
using Assets.Scripts.Combat.Conditions;

namespace Assets.Scripts.AI.HeroAI
{
    /// <summary>
    /// A smart AI for the knight. Used only by the combat simulator.
    /// Tries to keep enemies focused on himself.
    /// </summary>
    public class KnightAi : HeroAiBase
    {
        /// <summary>
        /// Hacky solution - to ensure both the Cleric and the Knight don't choose the same target, knight AI skips the first frame.
        /// Cleric can then put the target to sleep and the knight's AI will ignore it.
        /// </summary>
        bool didSkipFirstUpdate = false;
        /// <summary>
        /// How hurt must an ally be to consider using taunt to save him.
        /// </summary>
        protected const float TauntHealthThreshold = 0.1f;
        /// <summary>
        /// How dangerous must an enemy be to consider using the single target taunt on him and keeping him taunted forever.
        /// </summary>
        protected const float SingleTauntThreshold = 3f;
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void Update()
        {
            base.Update();
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void Start()
        {
            base.Start();
        }
        /// <summary>
        /// If there is a reason to use major taunt, use it.
        /// Otherwise if there is a powerful enough enemy present, try to taunt him.
        /// Otherwise to do basic hero AI.
        /// </summary>
        /// <returns>True if the knight does something, otherwise false.</returns>
        protected override bool TryDoAction()
        {
            if (!didSkipFirstUpdate)
            {
                // Ok, this is really, really hacky. But basically, cleric will on his first frame choose an enemy to put to sleep and enemy cleric might target the ranger.
                // Both of these have effect on the AI.
                // Not pretty, but as this is only for the combat simulator AI, it's probably fine.
                didSkipFirstUpdate = true;
                return false;
            }
            // Knight AI is basically mainly about taunting enemies and keeping them on himself.
            // As the friendly skill is highly situational, we will only use the other two skills.
            // Major taunt if enemy cleric painted our ranger as an target or if an ally is in danger.
            // Target specific taunt if there is only one really dangerous enemy to keep in check.
            if ((IsRangerTargeted() || IsAllyInDanger()) && TryUseSkill(ControlledCombatant, Knight.SelfTargetSkill))
            {
                return true;
            }
            var mostPowerfulTarget = GetMostDangerousTarget(dangerousnessThreshold: SingleTauntThreshold);
            if (mostPowerfulTarget != null && TryUseSkill(mostPowerfulTarget, Knight.EnemyTargetSkill))
            {
                return true;
            }
            // No skills to be used, to standard hero stuff.
            return base.TryDoAction();
        }
        /// <summary>
        /// Checks if there is an ally that is hurt enough to consider using the Taunt skill.
        /// </summary>
        /// <returns>True if there is such an ally, otherwise false.</returns>
        protected bool IsAllyInDanger()
        {
            // Ally is in danger if his health is below a specified threshold. However, we do not use t
            var allies = new List<CombatantBase> { Ranger, Cleric }.Where(ally => !ally.IsDown);
            return allies.Any(ally =>
                ally.HitPoints / ally.MaxHitpoints < TauntHealthThreshold
            );
        }
        /// <summary>
        /// Checks whether the enemy cleric targeted the ranger.
        /// </summary>
        /// <returns>True if the ranger is being targeted by the enemy cleric, otherwise false.</returns>
        protected bool IsRangerTargeted()
        {
            var aliveEnemies = CombatantsManager.GetEnemies(onlyAlive: true);
            return aliveEnemies.Any(enemy =>
            {
                var targetCondition = enemy.GetComponent<ForcedTargetCondition>();
                return targetCondition != null && targetCondition.ForcedTarget == Ranger;
            });
        }
    }
}