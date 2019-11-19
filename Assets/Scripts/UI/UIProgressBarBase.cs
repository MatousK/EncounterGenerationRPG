using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class UIProgressBarBase: MonoBehaviour
{
    protected CombatantBase RepresentedCombatant;
    protected virtual void Start()
    {
        RepresentedCombatant = GetComponentInParent<CombatantBase>();
        UpdateIndicators();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        UpdateIndicators();
        // Ensure that if the parent is flipped, this is flipped too, so in the end, nothing is flipped.
        var xScale = transform.parent.localScale.x < 0 ? -1 : 0;
        transform.localScale = new Vector3(transform.parent.localScale.x, 1, 1);
    }
    protected abstract void UpdateIndicators();
}