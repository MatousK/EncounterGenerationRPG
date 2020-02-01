using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class AIActionSelectionManager: MonoBehaviour
{
    /// <summary>
    /// The first of these classes that can specify a target will win and specify the current target.
    /// </summary>
    public List<AIActionSelectionMethodBase> OrderedActionSelectionMethods;

    public Skill ChooseAction(CombatantBase target)
    {
        foreach (var actionSelection in OrderedActionSelectionMethods)
        {
            if (actionSelection.ShouldSelectAction(target) && actionSelection.ActionSkill != null && actionSelection.ActionSkill.CanUseSkill())
            {
                return actionSelection.ActionSkill;
            }
        }
        return null;
    }
}