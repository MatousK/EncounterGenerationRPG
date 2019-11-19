using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class RangedBasicAttack: ProjectileAttack
{
    RangedBasicAttack()
    {
        BlocksManualMovement = false;
        isBasicAttack = true;
    }
    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }
}