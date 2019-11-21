using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : UIProgressBarBase
{

    public UIBar TotalMaxHitPointsIndicator;
    public UIBar CurrentMaxHitPointsIndicator;
    public UIBar CurrentHitPointsIndicator;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected override void UpdateIndicators()
    {
        CurrentMaxHitPointsIndicator.Percentage = (float)RepresentedCombatant.MaxHitpoints / RepresentedCombatant.TotalMaxHitpoints;
        CurrentHitPointsIndicator.Percentage = (float)RepresentedCombatant.HitPoints / RepresentedCombatant.TotalMaxHitpoints;
    }
}
