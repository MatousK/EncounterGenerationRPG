using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// A combatant with this condition cannot take any actions.
/// </summary>
class StunCondition: ConditionBase
{ 
    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void StartCondition()
    {
        base.StartCondition();
        foreach (var skill in GetComponentsInChildren<Skill>())
        {
            skill.TryStopSkill();
        }
    }
}