using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    CombatantBase RepresentedCombatant;

    public UIBar TotalMaxHitPointsIndicator;
    public UIBar CurrentMaxHitPointsIndicator;
    public UIBar CurrentHitPointsIndicator;

    private void Start()
    {
        RepresentedCombatant = GetComponentInParent<CombatantBase>();
        UpdateIndicators();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateIndicators();
    }

    private void UpdateIndicators()
    {
        CurrentMaxHitPointsIndicator.Percentage = (float)RepresentedCombatant.MaxHitpoints / RepresentedCombatant.TotalMaxHitpoints;
        CurrentHitPointsIndicator.Percentage = (float)RepresentedCombatant.HitPoints / RepresentedCombatant.TotalMaxHitpoints;
    }
}
