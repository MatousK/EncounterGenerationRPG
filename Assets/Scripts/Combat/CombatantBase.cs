using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/// <summary>
/// The base class for all entities that can be involved in combat, i.e. both monsters and player characters.
/// </summary>
public class CombatantBase : MonoBehaviour
{
    /// <summary>
    /// This event is raised whenever the combatant is defeated.
    /// </summary>
    public event EventHandler CombatantDied;
    /// <summary>
    /// This event is raised whenever the combatant is damaged.
    /// </summary>
    public event EventHandler<int> TookDamage;
    /// <summary>
    /// This event is raised whenever the combatant is healed.
    /// </summary>
    public event EventHandler<int> HealedDamage;
    /// <summary>
    /// If true, combat damage deals damage to max hitpoints directly.
    /// If false, normal hit points are depleted first.
    /// </summary>
    [NonSerialized]
    public bool DamageMaxHitPointsDirectly;
    /// <summary>
    /// What was the total cooldown of the last skill used.
    /// </summary>
    public float? LastSkillCooldown { get; protected set; }
    /// <summary>
    ///  What is the remaining cooldown of the last skill used.
    /// </summary>
    public float? LastSkillRemainingCooldown { get; protected set; }
    /// <summary>
    /// Attributes of the character that can change the character's attack and defense capabilities.
    /// </summary>
    public CombatantAttributes Attributes = new CombatantAttributes();
    /// <summary>
    /// How many hitpoints can the combatant have, i.e. size of the healthbar.
    /// </summary>
    public int TotalMaxHitpoints;
    /// <summary>
    /// How many hitpoints does the combatant have.
    /// </summary>
    public float HitPoints { get; protected set; }
    /// <summary>
    ///  Current maximum hitpoints, i.e. value to which the combatant can be healed.
    /// </summary>
    public int MaxHitpoints { get; protected set; }
    /// <summary>
    /// All skills this combatant possesses, including the basic attack.
    /// </summary>
    public Skill[] CombatantSkills { get; protected set; }
    /// <summary>
    /// If true, the character is defeated. They might still theoretically be resurrected, that's why we use Down insteadof Dead.
    /// </summary>
    public bool IsDown
    {
        get
        {
            return MaxHitpoints <= 0;
        }
    }

    protected CombatantsManager combatantsManager;

    public virtual bool IsBlockingSkillInProgress()
    {
        return CombatantSkills.Any(skill => skill.IsBeingUsed() && skill.BlocksOtherSkills && !skill.isBasicAttack);
    }

    public bool IsMoving()
    {
        return GetComponent<MovementController>()?.IsMoving ?? false;
    }
    /// <summary>
    /// Returns true if the combatant currently has orders from AI or from the player.
    /// </summary>
    /// <returns></returns>
    public bool IsDoingNonAutoAttackAction()
    {
        return IsBlockingSkillInProgress() || IsMoving();
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
            MaxHitpoints += (int)(HitPoints);
            HitPoints = 0;
        }
        // Max hitpoints should never fall below zero.
        MaxHitpoints = MaxHitpoints >= 0 ? MaxHitpoints : 0;
        TookDamage?.Invoke(this, damage);
        if (IsDown)
        {
            GetComponent<Animator>().SetBool("Dead", true);
            CombatantDied?.Invoke(this, new EventArgs());
        }
    }

    public virtual void HealDamage(int healAmount, CombatantBase fromCombatant, bool withDefaultAnimation = true)
    {
        healAmount = (int)(healAmount * Attributes.ReceivedHealingMultiplier * (fromCombatant?.Attributes?.DealtHealingMultiplier ?? 1));
        HitPoints = HitPoints + healAmount > MaxHitpoints ? MaxHitpoints : HitPoints + healAmount;
        HealedDamage?.Invoke(this, healAmount);
        if (withDefaultAnimation)
        {
            GetComponent<Animator>().SetTrigger("Healed");
        }
    }

    public virtual void StartCooldown(float cooldownTime)
    {
        LastSkillCooldown = cooldownTime;
        LastSkillRemainingCooldown = cooldownTime;
    }

    protected virtual void Awake()
    {
        MaxHitpoints = TotalMaxHitpoints;
        HitPoints = TotalMaxHitpoints;
        CombatantSkills = GetComponents<Skill>();
        combatantsManager = FindObjectOfType<CombatantsManager>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        UpdateSkillCooldown();
        ApplyHealthRegeneration();
    }

    public void DeathAnimationFinished()
    {
        Destroy(gameObject);
    }

    private void ApplyHealthRegeneration()
    {
        float regenerationRate = combatantsManager.IsCombatActive ? Attributes.CombatHealthRegeneration : Attributes.OutOfCombatHealthRegeneration;
        float regeneratedHP = regenerationRate * Time.deltaTime;
        HitPoints = Math.Min(MaxHitpoints, HitPoints + regeneratedHP);
    }

    private void UpdateSkillCooldown()
    {
        if (LastSkillRemainingCooldown.HasValue)
        {
            LastSkillRemainingCooldown -= Time.deltaTime;
        }
    }
}
