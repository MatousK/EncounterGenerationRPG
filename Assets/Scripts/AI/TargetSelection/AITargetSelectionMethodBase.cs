using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class AITargetSelectionMethodBase: MonoBehaviour
{
    protected CombatantBase representedCombatant;

    protected virtual void Awake()
    {
        representedCombatant = transform.parent.gameObject.GetComponent<CombatantBase>();
    }
    public abstract bool TryGetTarget(ref CombatantBase target);
}