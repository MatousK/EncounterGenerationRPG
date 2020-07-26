using Assets.Scripts.Combat;
using Assets.Scripts.Extension;

namespace Assets.Scripts.AI.MonsterAI
{
    /// <summary>
    /// AI for snipers. They select their targets randomly if not forced. 
    /// The special behavior of snipers is that elite and boss snipers lock to their random targets, shooting them until they die.
    /// </summary>
    public class SniperAi: MonsterAiBase
    {
        /// <summary>
        /// Sniper lock to a random target and fire at it until it is dead.
        /// </summary>
        private CombatantBase lockedRandomTarget;
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
        /// Forced target is the priority as always.
        /// Regular snipers will always return a random target.
        /// Elite and boss snipers will select one random hero, lock onto him and then always return that locked hero until he dies.
        /// </summary>
        /// <returns>The target of this monster, or null if no heroes are alive.</returns>
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
