using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// There are no conditions for this action selection, it always returns true.
/// </summary>
public class AIAlwaysActionSelection : AIActionSelectionMethodBase
{
    protected override void Awake()
    {
        base.Awake();
    }
    public override bool ShouldSelectAction(CombatantBase target)
    {
        return true;
    }
}