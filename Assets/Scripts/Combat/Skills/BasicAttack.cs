using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class BasicAttack : Attack
{
    BasicAttack()
    {
        BlocksManualMovement = false;
        IsBasicAttack = true;
    }
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Update()
    {
        base.Update();
    }
}
