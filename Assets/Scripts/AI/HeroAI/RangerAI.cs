using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public class RangerAI: HeroAIBase
{
    bool didSkipFirstUpdate = false;
    const float SniperShotMonsterDangerThreshold = 3;
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
            // Hacky way to give the cleric the time to put an enemy to sleep.
            didSkipFirstUpdate = true;
            return;
        }
        // Ranger should, in order:
        // Use the sniper shot if there is a high priority target there.
        // Use the personal skill if there is not a high priority target.
        // Do not use friendly skill, there is no easily described way to use this.
        var sniperShotTarget = GetMostDangerousTarget(dangerousnessThreshold: SniperShotMonsterDangerThreshold);
        if (sniperShotTarget != null && TryUseSkill(sniperShotTarget, Ranger.EnemyTargetSkill))
        {
            return;
        }
        if (TryUseSkill(ControlledCombatant, Ranger.SelfTargetSkill))
        {
            return;
        }
        // If we can oneshot an enemy, do it, we will probably save a lot in healing.
        var oneShotTarget = GetOneShotEnemy();
        if (oneShotTarget != null && TryUseSkill(oneShotTarget, BasicAttack))
        {
            return;
        }
        base.OnActionRequired();
    }

    // Retrieve an enemy we can kill with one shot.
    CombatantBase GetOneShotEnemy()
    {
        var oneShotEnemies = CombatantsManager.GetEnemies(onlyAlive: true).Where(enemy => enemy.HitPoints < ControlledCombatant.Attributes.DealtDamageMultiplier);
        return oneShotEnemies.Any() ? oneShotEnemies.Aggregate((enemy1, enemy2) => GetMonsterDangerScore(enemy1) > GetMonsterDangerScore(enemy2) ? enemy1 : enemy2) : null;
    }
}
