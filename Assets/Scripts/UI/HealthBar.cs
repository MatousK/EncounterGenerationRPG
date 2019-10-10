using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public float TotalMaxHitPoints = 1;
    public float CurrentMaxHitPoints = 1;
    public float CurrentHitPoints = 1;

    public UIBar TotalMaxHitPointsIndicator;
    public UIBar CurrentMaxHitPointsIndicator;
    public UIBar CurrentHitPointsIndicator;

    private void Start()
    {
        UpdateIndicators();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateIndicators();
    }

    private void UpdateIndicators()
    {
        CurrentMaxHitPointsIndicator.Percentage = CurrentMaxHitPoints / TotalMaxHitPoints;
        CurrentHitPointsIndicator.Percentage = CurrentHitPoints / TotalMaxHitPoints;
    }
}
