using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class HealOther : TargetedGestureSkill
{
    public int HealAmount = 100;
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
    protected override void ApplySkillEffects(object sender, EventArgs e)
    {
        Target?.HealDamage(HealAmount, selfCombatant);
    }
}