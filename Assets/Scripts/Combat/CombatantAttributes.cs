using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class CombatantAttributes
{
    public float DealtDamageMultiplier = 1;
    public float ReceivedDamageMultiplier = 1;
    public float AttackSpeedMultiplier = 1;
    public float MovementSpeedMultiplier = 1;
    public float RangeMultiplier = 1;
    public float DealtHealingMultiplier = 1;
    public float ReceivedHealingMultiplier = 1;
}
