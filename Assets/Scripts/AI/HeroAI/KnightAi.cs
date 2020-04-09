using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Combat;
using Assets.Scripts.Combat.Conditions;

namespace Assets.Scripts.AI.HeroAI
{
    public class KnightAi : HeroAiBase
    {
        bool didSkipFirstUpdate = false;
        protected const float TauntHealthThreshold = 0.1f;
        protected const float SingleTauntThreshold = 3f;
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
            if (!didSkipFirstUpdate)
            {
                // Ok, this is really, really hacky. But basically, cleric will on his first frame choose an enemy to put to sleep and enemy cleric might target the ranger.
                // Both of these have effect on the AI.
                // Not pretty, but as this is only for the combat simulator AI, it's probably fine.
                didSkipFirstUpdate = true;
                return;
            }
            // Knight AI is basically mainly about taunting enemies and keeping them on himself.
            // As the friendly skill is highly situational, we will only use the other two skills.
            // Major taunt if enemy cleric painted our ranger as an target or if an ally is in danger.
            // Target specific taunt if there is only one really dangerous enemy to keep in check.
            if ((IsRangerTargeted() || IsAllyInDanger()) && TryUseSkill(ControlledCombatant, Knight.SelfTargetSkill))
            {
                return;
            }
            var mostPowerfulTarget = GetMostDangerousTarget(dangerousnessThreshold: SingleTauntThreshold);
            if (mostPowerfulTarget != null && TryUseSkill(mostPowerfulTarget, Knight.EnemyTargetSkill))
            {
                return;
            }
            // No skills to be used, to standard hero stuff.
            base.OnActionRequired();
        }

        protected bool IsAllyInDanger()
        {
            // Ally is in danger if his health is below a specified threshold. However, we do not use t
            var allies = new List<CombatantBase> { Ranger, Cleric }.Where(ally => !ally.IsDown);
            return allies.Any(ally =>
                ally.HitPoints / ally.MaxHitpoints < TauntHealthThreshold
            );
        }

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