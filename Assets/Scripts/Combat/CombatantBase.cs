using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CombatantBase : MonoBehaviour
{
    public event EventHandler CombatantDied;
    public event EventHandler<int> TookDamage;
    public event EventHandler<int> HealedDamage;
    /// <summary>
    /// If true, combat damage deals damage to max hitpoints directly.
    /// If false, normal hit points are depleted first.
    /// </summary>
    [NonSerialized]
    public bool DamageMaxHitPointsDirectly;
    public CombatantAttributes Attributes = new CombatantAttributes();
    // How many hitpoints can the combatant have, i.e. size of the healthbar.
    public int TotalMaxHitpoints;
    // How many hitpoints does the combatant have
    public int HitPoints { get; protected set; }
    // Current maximum hitpoints, i.e. value to which the combatant can be healed.
    public int MaxHitpoints { get; protected set; }
    public Skill[] CombatantSkills { get; protected set; }

    HealthBar healthBar;

    public bool IsDown
    {
        get
        {
            return MaxHitpoints <= 0;
        }
    }

    public virtual bool IsSkillUsageBlocked()
    {
        return CombatantSkills.Any(skill => skill.IsBeingUsed() && skill.BlocksOtherSkills && !IsBasicAttack(skill));
    }

    public virtual bool IsManualMovementBlocked()
    {
        return CombatantSkills.Any(skill => skill.IsBeingUsed() && skill.BlocksManualMovement);
    }

    public virtual void TakeDamage(int damage, CombatantBase FromCombatant)
    {
        damage = (int)(damage * Attributes.ReceivedDamageMultiplier * (FromCombatant?.Attributes?.DealtDamageMultiplier ?? 1));
        HitPoints -= damage;
        if (DamageMaxHitPointsDirectly)
        {
            MaxHitpoints -= damage;
        }
        if (HitPoints < 0)
        {
            // Once hitpoints are depleted, max HP should start depleting.
            MaxHitpoints += HitPoints;
            HitPoints = 0;
        }
        // Max hitpoints should never fall below zero.
        MaxHitpoints = MaxHitpoints >= 0 ? MaxHitpoints : 0;
        UpdateHealthbar();
        TookDamage?.Invoke(this, damage);
        if (IsDown)
        {
            GetComponent<Animator>().SetBool("Dead", true);
            CombatantDied?.Invoke(this, new EventArgs());
        }
    }

    public virtual void HealDamage(int healAmount, CombatantBase FromCombatant)
    {
        healAmount = (int)(healAmount * Attributes.ReceivedHealingMultiplier * (FromCombatant?.Attributes?.DealtHealingMultiplier ?? 1));
        HitPoints = HitPoints + healAmount > MaxHitpoints ? MaxHitpoints : HitPoints + healAmount;
        UpdateHealthbar();
        HealedDamage?.Invoke(this, healAmount);
    }

    public virtual bool IsBasicAttack(Skill skill)
    {
        return skill is BasicAttack;
    }

    protected virtual void Start()
    {
        MaxHitpoints = TotalMaxHitpoints;
        HitPoints = TotalMaxHitpoints;
        healthBar = GetComponentInChildren<HealthBar>();
        UpdateHealthbar();
        CombatantSkills = GetComponents<Skill>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
    }

    void UpdateHealthbar()
    {
        healthBar.TotalMaxHitPoints = TotalMaxHitpoints;
        healthBar.CurrentMaxHitPoints = MaxHitpoints;
        healthBar.CurrentHitPoints = HitPoints;
    }

    public void DeathAnimationFinished()
    {
        Destroy(gameObject);
    }
}
