using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class AIDistanceActionSelection: AIActionSelectionMethodBase
{
    public DistanceComparison ConstantComparison;
    public float DistanceConstant;

    public override bool ShouldSelectAction(CombatantBase target)
    {
        var distanceToTarget = representedCombatant.GetComponent<Collider2D>().Distance(target.GetComponent<Collider2D>());
        switch (ConstantComparison)
        {
            case DistanceComparison.LessThan:
                return distanceToTarget.distance < DistanceConstant;
            case DistanceComparison.MoreThan:
                return distanceToTarget.distance > DistanceConstant;
        }
        Debug.Assert(false, "Enum should be exhaustive");
        return false;
    }
}

public enum DistanceComparison
{
    LessThan,
    MoreThan
}