using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : CombatantBase
{
    protected override void Start()
    {
        base.Start();
        combatantsManager.Enemies.Add(this);
        DamageMaxHitPointsDirectly = true;
    }
    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public override void TakeDamage(int damage, CombatantBase FromCombatant)
    {
        base.TakeDamage(damage, FromCombatant);
        if (IsDown)
        {
            combatantsManager.Enemies.Remove(this);
        }
    }
}
