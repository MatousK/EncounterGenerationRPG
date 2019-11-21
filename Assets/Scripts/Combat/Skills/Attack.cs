using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class Attack : TargetedSkill
{
    /// <summary>
    /// How much damage does the attack do per hit.
    /// </summary>
    public int DamagePerHit = 1;

    public Attack()
    {
        SkillAnimationName = "Attacking";
    }

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void ApplySkillEffects(object sender, EventArgs e)
    {
        Target?.TakeDamage(DamagePerHit, selfCombatant);
    }
}
