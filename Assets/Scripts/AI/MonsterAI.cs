using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class MonsterAI: MonoBehaviour
{
    private AIActionSelectionManager actionSelectionManager;
    private AITargetSelectionManager targetSelectionManager;
    private CombatantBase controlledCombatant;

    private void Awake()
    {
        actionSelectionManager = GetComponent<AIActionSelectionManager>();
        targetSelectionManager = GetComponent<AITargetSelectionManager>();
        controlledCombatant = GetComponentInParent<CombatantBase>();
    }

    private void Update()
    {
        if (!controlledCombatant.IsBlockingSkillInProgress(true))
        {
            var currentTarget = targetSelectionManager.ChooseTarget();
            if (currentTarget == null)
            {
                // Noone to target right now, do nothing.
                return;
            }
            var currentSkill = actionSelectionManager.ChooseAction(currentTarget);
            if (currentSkill == null)
            {
                // Nothing to use right now.
                return;
            }
            if (currentSkill is TargetedSkill)
            {
                (currentSkill as TargetedSkill).UseSkillOn(currentTarget);
            }
            else if (currentSkill is PersonalSkill)
            {
                (currentSkill as PersonalSkill).ActivateSkill();
            } else
            {
                Debug.Assert(false, "Do not know how to activate skill of type " + currentSkill.ToString());
            }
        }
    }
}