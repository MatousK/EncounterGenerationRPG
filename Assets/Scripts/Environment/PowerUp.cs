using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public float AttackModifierAddition;
    public float AttackModifierMultiplier = 1;
    public float DefenseModifierSubstraction;
    public float DefenseModifierMultiplier = 1;
    public float TotalMaxHPAddition;
    public float TotalMaxHPMultiplier = 1;
    public float HealedMaxHP;
    public void ApplyPowerup(Hero forHero)
    {
        forHero.Attributes.DealtDamageMultiplier += AttackModifierAddition;
        forHero.Attributes.DealtDamageMultiplier *= AttackModifierMultiplier;

        forHero.Attributes.ReceivedDamageMultiplier -= DefenseModifierSubstraction;
        forHero.Attributes.ReceivedDamageMultiplier *= DefenseModifierMultiplier;

        var oldTotalMaxHP = forHero.TotalMaxHitpoints;
        forHero.TotalMaxHitpoints += TotalMaxHPAddition;
        forHero.TotalMaxHitpoints *= TotalMaxHPMultiplier;

        var toAddMaxHP = forHero.TotalMaxHitpoints - oldTotalMaxHP + HealedMaxHP;
        forHero.MaxHitpoints += toAddMaxHP;
        if (toAddMaxHP != 0)
        {
            if (forHero.MaxHitpoints > forHero.TotalMaxHitpoints)
            {
                forHero.MaxHitpoints = forHero.TotalMaxHitpoints;
            }
            forHero.HealDamage((int)toAddMaxHP, null, true);
        }
    }
}