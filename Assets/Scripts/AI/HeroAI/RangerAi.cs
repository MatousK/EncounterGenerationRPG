using System.Linq;
using Assets.Scripts.Combat;

namespace Assets.Scripts.AI.HeroAI
{
    /// <summary>
    /// Smart AI for the ranger, used only for combat simulator.
    /// Tries to do as much damage as possible.
    /// If there are sufficiently powerful enemies, it will try to kill them with sniper shot.
    /// If there are not, it will try to kill enemies it can one shot.
    /// If there are none, just start shooting enemies at random.
    /// </summary>
    public class RangerAi: HeroAiBase
    {
        /// <summary>
        /// Hacky solution, used to indicate whether the ranger skipped the first update.
        /// He should do that, because he needs to know which enemy will the allied cleric put to sleep.
        /// </summary>
        bool didSkipFirstUpdate = false;
        /// <summary>
        /// If some enemy is more dangerous than this, it will try to kill him with sniper shot.
        /// </summary>
        const float SniperShotMonsterDangerThreshold = 3;
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
        /// Called when the ranger should do something.
        /// He will try to kill the most dangerous enemies with sniper shot.
        /// Otherwise it will trigger Rapid Stance and start killing enemies, preferably those he can one shot.
        /// </summary>
        /// <returns> True if some action was returned, otherwise false.</returns>
        protected override bool TryDoAction()
        {
            if (!didSkipFirstUpdate)
            {
                // Hacky way to give the cleric the time to put an enemy to sleep.
                didSkipFirstUpdate = true;
                return false;
            }
            // Ranger should, in order:
            // Use the sniper shot if there is a high priority target there.
            // Use the personal skill if there is not a high priority target.
            // Do not use friendly skill, there is no easily described way to use this.
            var sniperShotTarget = GetMostDangerousTarget(dangerousnessThreshold: SniperShotMonsterDangerThreshold);
            if (sniperShotTarget != null && TryUseSkill(sniperShotTarget, Ranger.EnemyTargetSkill))
            {
                return true;
            }
            if (TryUseSkill(ControlledCombatant, Ranger.SelfTargetSkill))
            {
                return true;
            }
            // If we can oneshot an enemy, do it, we will probably save a lot in healing.
            var oneShotTarget = GetOneShotEnemy();
            if (oneShotTarget != null && TryUseSkill(oneShotTarget, BasicAttack))
            {
                return true;
            }
            return base.TryDoAction();
        }

        /// <summary>
        /// Retrieve an enemy we can kill with one shot
        /// </summary>
        /// <returns> The enemy we can kill in one shot, otherwise false.</returns>
        CombatantBase GetOneShotEnemy()
        {
            var oneShotEnemies = CombatantsManager.GetEnemies(onlyAlive: true).Where(enemy => enemy.HitPoints < ControlledCombatant.Attributes.DealtDamageMultiplier);
            return oneShotEnemies.Any() ? oneShotEnemies.Aggregate((enemy1, enemy2) => GetMonsterDangerScore(enemy1) > GetMonsterDangerScore(enemy2) ? enemy1 : enemy2) : null;
        }
    }
}
