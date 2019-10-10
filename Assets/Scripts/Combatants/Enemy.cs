using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : CombatantBase
{
    protected override void Start()
    {
        base.Start();
    }
    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public override void DealDamage(int damage)
    {
        // Monsters deplete max HP directly, they do not deplete normal hit points first.
        MaxHitpoints -= damage;
        base.DealDamage(damage);
    }
}
