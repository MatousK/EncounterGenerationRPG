using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class SniperAI: MonsterAIBase
{
    // Sniper lock to a random target and fire at it until it is dead.
    private CombatantBase LockedRandomTarget;
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
        if (LockedRandomTarget == null || LockedRandomTarget.IsDown)
        {
            var opponents = CombatantsManager.GetOpponentsFor(ControlledCombatant, onlyAlive: true);
            var randomTarget = opponents.GetWeightedRandomElementOrDefault(opponent => 1);
            // Regular monsters do not lock targets and instead shoot all over the place randomly.
            if (((Monster)ControlledCombatant).Rank != MonsterRank.Regular)
            {
                LockedRandomTarget = randomTarget;
            } 
            else 
            {
                return randomTarget;
            }
        }
        return LockedRandomTarget;
    }
}
