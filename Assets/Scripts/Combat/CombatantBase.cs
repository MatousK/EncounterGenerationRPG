using System;
using System.Linq;
using Assets.Scripts.Combat.Skills;
using Assets.Scripts.Movement;
using Assets.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts.Combat
{
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
        /// If true, combat damage deals damage to max hit points directly.
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
        public float TotalMaxHitpoints;
        /// <summary>
        /// How many hitpoints does the combatant have.
        /// </summary>
        public float HitPoints { get; protected set; }
        /// <summary>
        ///  Current maximum hitpoints, i.e. value to which the combatant can be healed and can be restored by items.
        /// </summary>
        public float MaxHitpoints;
        /// <summary>
        /// All skills this combatant possesses, including the basic attack.
        /// </summary>
        public Skill[] CombatantSkills { get; protected set; }
        /// <summary>
        /// If true, the character is defeated. They might still theoretically be resurrected, that's why we use Down instead of Dead.
        /// </summary>
        public bool IsDown => MaxHitpoints <= 0;
        /// <summary>
        /// If true, this character cannot currently be damaged, targeted or killed.
        /// </summary>
        public bool IsInvincible;
        /// <summary>
        /// If true, this character can be a target of skills and abilities.
        /// </summary>
        public bool CanBeTargeted => !IsDown && !IsInvincible;
        /// <summary>
        /// The class which has references to all combatants in the game.
        /// </summary>
        protected CombatantsManager CombatantsManager;
        /// <summary>
        /// Checks if the combatant is currently using a skill that blocks other skills.
        /// By blocking we mean that no other skill can be used while using this skill.
        /// Usually personal skills are non-blocking, as attacking is possible while an aura is active.
        /// </summary>
        /// <param name="isBasicAttackBlocking">If true, basic attacks are considered to be blocking, otherwise they are not. </param>
        /// <returns>True if a blocking skill is in progress.</returns>
        public bool IsBlockingSkillInProgress(bool isBasicAttackBlocking)
        {
            return CombatantSkills.Any(skill => skill.IsBeingUsed() && skill.BlocksOtherSkills && (!skill.IsBasicAttack || isBasicAttackBlocking));
        }
        /// <summary>
        /// Helper method to check whether the combatant is currently moving.
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// If true, the combatant's controller cannot order him to move, he's probably using a skill forbidding movement.
        /// </summary>
        /// <returns></returns>
        public virtual bool IsManualMovementBlocked()
        {
            return CombatantSkills.Any(skill => skill.IsBeingUsed() && skill.BlocksManualMovement);
        }
        /// <summary>
        /// Decrease HP of the combatant. Always use this skill instead of directly reducing hit points.
        /// This skill modifies the damage by relevant multipliers, decreases max hit points when necessary,
        /// raises events and shows damage indicators.
        /// </summary>
        /// <param name="damage">The amount of damage this combatant received.</param>
        /// <param name="fromCombatant">The combatant from whom the damage was received.</param>
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
                // Once hit points are depleted, max HP should start depleting.
                MaxHitpoints += (int)(HitPoints);
                HitPoints = 0;
            }
            // Max hit points should never fall below zero.
            MaxHitpoints = MaxHitpoints >= 0 ? MaxHitpoints : 0;
            TookDamage?.Invoke(this, damage);
            if (IsDown)
            {
                GetComponent<Animator>().SetBool("Dead", true);
                CombatantDied?.Invoke(this, new EventArgs());
            }
            GetComponent<FloatingTextController>().ShowDamageIndicator(damage);
        }
        /// <summary>
        /// Increases hit points of the this combatant, though never over <see cref="MaxHitpoints"/>
        /// Always use this skill instead of directly increasing hit points variable.
        /// This skill modifies the healed amount by relevant modifiers, raises an event,
        /// shows heal indicator a triggers a relevant animation.
        /// </summary>
        /// <param name="healAmount">The amount of hit points that should be restored.</param>
        /// <param name="fromCombatant">The combatant who triggered the heal.</param>
        /// <param name="withDefaultAnimation">If true, this combatant will play an animation indicating he was healed.</param>
        public virtual void HealDamage(float healAmount, CombatantBase fromCombatant, bool withDefaultAnimation = true)
        {
            healAmount *= Attributes.ReceivedHealingMultiplier;
            if (fromCombatant != null)
            {
                healAmount *= (fromCombatant.Attributes.DealtHealingMultiplier);
            }
            int healAmountInt = (int)healAmount;
            HitPoints = HitPoints + healAmountInt > MaxHitpoints ? MaxHitpoints : HitPoints + healAmountInt;
            HealedDamage?.Invoke(this, healAmountInt);
            if (withDefaultAnimation)
            {
                GetComponent<Animator>().SetTrigger("Healed");
            }
            GetComponent<FloatingTextController>().ShowHealingIndicator(healAmountInt);
        }
        /// <summary>
        /// Starts a cooldown on all non basic attack skills, making them unusable for a while.
        /// </summary>
        /// <param name="cooldownTime">How long should the cool down last.</param>
        public virtual void StartCooldown(float cooldownTime)
        {
            LastSkillCooldown = cooldownTime;
            LastSkillRemainingCooldown = cooldownTime;
        }
        /// <summary>
        /// Called when the script instance is being loaded.
        /// Initializes references to dependencies in this object.
        /// Also initializes hit points.
        /// </summary>
        protected virtual void Awake()
        {
            MaxHitpoints = TotalMaxHitpoints;
            HitPoints = TotalMaxHitpoints;
            CombatantSkills = GetComponentsInChildren<Skill>().Where(skill => skill.gameObject.activeSelf).ToArray();
        }
        /// <summary>
        /// Called before the first Update. Children are expected to register themselves to the <see cref="CombatantsManager"/>.
        /// </summary>
        protected virtual void Start()
        {
            CombatantsManager = FindObjectOfType<CombatantsManager>();
        }

        /// <summary>
        /// Called every update. Updates cooldowns, regeneration and stops doing everything if dead.
        /// </summary>
        protected virtual void Update()
        {
            if (IsDown)
            {
                foreach (var skill in CombatantSkills)
                {
                    if (skill.IsBeingUsed())
                    {
                        skill.TryStopSkill();
                    }
                }
            }
            UpdateSkillCooldown();
            ApplyHealthRegeneration();
            ResetCooldownIfOutOfCombat();
        }
        /// <summary>
        /// Called when a monster's death animation finishes.
        /// Destroys the monster game object.
        /// </summary>
        public void DeathAnimationFinished()
        {
            Destroy(gameObject);
        }
        /// <summary>
        /// Sets new total max HP. Also sets max HP and HP to that same value.
        /// </summary>
        /// <param name="newTotalMaxHp">New total max HP value.</param>
        public void SetTotalMaxHp(float newTotalMaxHp)
        {
            TotalMaxHitpoints = newTotalMaxHp;
            MaxHitpoints = newTotalMaxHp;
            HitPoints = newTotalMaxHp;
        }
        /// <summary>
        /// Resets the cooldown on all skills, making them usable again.
        /// </summary>
        public void ResetCooldown()
        {
            LastSkillRemainingCooldown = 0;
            LastSkillCooldown = null;
        }
        /// <summary>
        /// If the character has regeneration powers, this applies them and restores HP.
        /// </summary>
        private void ApplyHealthRegeneration()
        {
            float regenerationRate = CombatantsManager.IsCombatActive ? Attributes.CombatHealthRegeneration : Attributes.OutOfCombatHealthRegeneration;
            float regeneratedHp = regenerationRate * Time.deltaTime;
            HitPoints = Math.Min(MaxHitpoints, HitPoints + regeneratedHp);
        }

        private void ResetCooldownIfOutOfCombat()
        {
            if (!CombatantsManager.IsCombatActive)
            {
                ResetCooldown();
            }
        }
        /// <summary>
        /// Update the remaining cooldown of skills by the delta time.
        /// </summary>
        private void UpdateSkillCooldown()
        {
            if (LastSkillRemainingCooldown.HasValue)
            {
                LastSkillRemainingCooldown -= Time.deltaTime;
            }
        }
    }
}
