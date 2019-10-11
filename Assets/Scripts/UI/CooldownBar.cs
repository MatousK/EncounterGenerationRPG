using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CooldownBar : MonoBehaviour
{
    CombatantBase RepresentedCombatant;

    public UIBar TotalCooldownIndicator;
    public UIBar CooldownProgressIndicator;

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
        // We do not show the indicator if we the cooldown is not active.
        if (!RepresentedCombatant.LastSkillRemainingCooldown.HasValue || !RepresentedCombatant.LastSkillCooldown.HasValue || RepresentedCombatant.LastSkillRemainingCooldown < 0)
        {
            TotalCooldownIndicator.gameObject.SetActive(false);
            CooldownProgressIndicator.gameObject.SetActive(false);
            return;
        }
        TotalCooldownIndicator.gameObject.SetActive(true);
        CooldownProgressIndicator.gameObject.SetActive(true);
        // We do not show time remaining, instead we show how much time has already passed.
        var progressPercentage = RepresentedCombatant.LastSkillRemainingCooldown.Value / RepresentedCombatant.LastSkillCooldown.Value;
        CooldownProgressIndicator.Percentage = 1f - progressPercentage;
    }
}
