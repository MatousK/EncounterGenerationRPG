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
    /// <summary>
    /// Checks if the combatant is currently using a skill that blocks other skills.
    /// By blocking we mean that no other skill can be used while using this skill.
    /// Usually personal skills are non-blocking, as attacking is possible while an aura is active.
    /// </summary>
    /// <param name="isAutoAttackBlocking">If true, basic attacks are considered to be blocking, otherwise they are not. </param>
    /// <returns>True if a blocking skill is in progress.</returns>
    public bool IsBlockingSkillInProgress(bool isBasicAttackBlocking)
    {
        return CombatantSkills.Any(skill => skill.IsBeingUsed() && skill.BlocksOtherSkills && (!skill.isBasicAttack || isBasicAttackBlocking));
    }

    public bool IsMoving()
    {
        return GetComponent<MovementController>().IsMoving;
    }
    /// <summary>
    /// Returns true if the combatant currently has orders from AI or from the player.
    /// </summary>
    /// <returns></returns>
    public bool IsDoingNonAutoAttackAction()
    {
        return IsBlockingSkillInProgress(false) || IsMoving();
    }

    public virtual bool IsManualMovementBlocked()
    {
        return CombatantSkills.Any(skill => skill.IsBeingUsed() && skill.BlocksManualMovement);
    }

    public virtual void TakeDamage(int damage, CombatantBase fromCombatant)
    {
        float damageFloat = damage * Attributes.ReceivedDamageMultiplier;
        if (fromCombatant != null)
        {
            damageFloat *= fromCombatant.Attributes.DealtDamageMultiplier;
        }
        damage = (int)damageFloat;
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
        float healAmountFloat = healAmount * Attributes.ReceivedHealingMultiplier;
        if (fromCombatant != null)
        {
            healAmountFloat *= (fromCombatant.Attributes.DealtHealingMultiplier);
        }
        healAmount = (int)healAmountFloat;
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
        CombatantSkills = GetComponentsInChildren<Skill>();
        combatantsManager = FindObjectOfType<CombatantsManager>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        UpdateSkillCooldown();
        ApplyHealthRegeneration();
        ResetCooldownIfOutOfCombat();
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

    private void ResetCooldownIfOutOfCombat()
    {
        if (!combatantsManager.IsCombatActive)
        {
            LastSkillRemainingCooldown = 0;
            LastSkillCooldown = null;
        }
    }

    private void UpdateSkillCooldown()
    {
        if (LastSkillRemainingCooldown.HasValue)
        {
            LastSkillRemainingCooldown -= Time.deltaTime;
        }
    }
}
