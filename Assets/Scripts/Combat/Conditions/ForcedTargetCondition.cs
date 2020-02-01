using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
/// <summary>
/// Helper condition for AI - specifies that the affected AI MUST attack the specified player or character for a specified duration.
/// When another target is called, the most recently applied condition will apply.
/// </summary>
class ForcedTargetCondition : ConditionBase
{
    /// <summary>
    /// The target this character must attack at all cost.
    /// </summary>
    public CombatantBase ForcedTarget;
    /// <summary>
    /// If true, once the character who cast this condition dies the condition will end.
    /// </summary>
    public bool StopTargetingOnceCallerDead = true;
    /// <summary>
    /// The character who forced this target.
    /// </summary>
    public CombatantBase TargetForcedBy;

    protected override void Update()
    {
        base.Update();
        if (StopTargetingOnceCallerDead && (TargetForcedBy != null && TargetForcedBy.IsDown))
        {
            EndCondition();
        }
    }

    protected override void Start() {
        base.Start();
    }
}