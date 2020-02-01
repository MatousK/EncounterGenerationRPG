using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Class that can select a target to be targeted by the AI.
/// </summary>
class AITargetSelectionManager: MonoBehaviour
{
    /// <summary>
    /// The first of these classes that can specify a target will win and specify the current target.
    /// </summary>
    public List<AITargetSelectionMethodBase> OrderedTargetSelectionMethods;

    public CombatantBase ChooseTarget()
    {
        CombatantBase toReturn = null;

        foreach (var targetSelectionMethod in OrderedTargetSelectionMethods)
        {
            if (targetSelectionMethod.TryGetTarget(ref toReturn))
            {
                return toReturn;
            }
        }
        return toReturn;
    }
}