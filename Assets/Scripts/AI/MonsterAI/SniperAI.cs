using Assets.Scripts.Combat;
using Assets.Scripts.Extension;

namespace Assets.Scripts.AI.MonsterAI
{
    public class SniperAi: MonsterAiBase
    {
        // Sniper lock to a random target and fire at it until it is dead.
        private CombatantBase lockedRandomTarget;
        protected override void Update()
        {
            base.Update();
        }

        protected override void Awake()
        {
            base.Awake();
        }

        protected override CombatantBase GetCurrentTarget()
        {
            if (ForcedTarget != null)
            {
                return ForcedTarget;
            }
            if (lockedRandomTarget == null || lockedRandomTarget.IsDown)
            {
                var opponents = CombatantsManager.GetOpponentsFor(ControlledCombatant, onlyAlive: true);
                var randomTarget = opponents.GetRandomElementOrDefault();
                // Regular monsters do not lock targets and instead shoot all over the place randomly.
                if (((Monster)ControlledCombatant).Rank != MonsterRank.Regular)
                {
                    lockedRandomTarget = randomTarget;
                } 
                else 
                {
                    return randomTarget;
                }
            }
            return lockedRandomTarget;
        }
    }
}
