using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThousandBlades : Attack
{
    // Start is called before the first frame update
    protected override void Start()
    {
        Cooldown = 6;
        Repetitions = 10;
        Speed = 5;
        DamagePerHit = 1;
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
}
