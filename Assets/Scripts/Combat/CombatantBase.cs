using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatantBase : MonoBehaviour
{
    public event EventHandler CombatantDied;
    public event EventHandler<int> TookDamage;
    public event EventHandler<int> HealedDamage;
    // How many hitpoints can the combatant have, i.e. size of the healthbar.
    public int TotalMaxHitpoints;
    // How many hitpoints does the combatant have
    public int HitPoints { get; protected set; }
    // Current maximum hitpoints, i.e. value to which the combatant can be healed.
    public int MaxHitpoints { get; protected set; }
    public TargetedSkill[] CharacterSkills { get; protected set; }

    HealthBar healthBar;

    public bool IsDown
    {
        get
        {
            return MaxHitpoints <= 0;
        }
    }

    public virtual void DealDamage(int damage)
    {
        HitPoints -= damage;
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

    public virtual void HealDamage(int healAmount)
    {
        HitPoints = HitPoints + healAmount > MaxHitpoints ? MaxHitpoints : HitPoints + healAmount;
        UpdateHealthbar();
        HealedDamage?.Invoke(this, healAmount);
    }

    protected virtual void Start()
    {
        MaxHitpoints = TotalMaxHitpoints;
        HitPoints = TotalMaxHitpoints;
        healthBar = GetComponentInChildren<HealthBar>();
        UpdateHealthbar();
        CharacterSkills = GetComponents<TargetedSkill>();
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
