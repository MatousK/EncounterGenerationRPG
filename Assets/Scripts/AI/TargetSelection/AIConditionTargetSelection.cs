using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class AIConditionTargetSelection : AITargetSelectionMethodBase
{
    protected override void Awake()
    {
        base.Awake();
    }
    public override bool TryGetTarget(ref CombatantBase target)
    {
        bool targetForcedByCondition = false;
        foreach (var condition in representedCombatant.GetComponent<ConditionManager>().ActiveConditions)
        {
            if (condition is SleepCondition)
            {
                // A sleep condition overrides all others, AI should do nothing at all.
                target = null;
                return true;
            }
            else if (condition is ForcedTargetCondition)
            {
                var forceTargetCondition = condition as ForcedTargetCondition;
                targetForcedByCondition = true;
                target = forceTargetCondition.ForcedTarget;
            }
        }
        return targetForcedByCondition;
    }
}
