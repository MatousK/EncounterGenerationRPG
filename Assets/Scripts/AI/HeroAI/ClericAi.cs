using System.Linq;
using Assets.Scripts.Combat;
using Assets.Scripts.Combat.Skills.Character.Cleric;
using Assets.Scripts.Movement;
using UnityEngine;

namespace Assets.Scripts.AI.HeroAI
{
    /// <summary>
    /// Smart AI class for the cleric which will use its skills tactically, putting dangerous enemies to sleep and healing allies when their injured.
    /// </summary>
    public class ClericAi: HeroAiBase
    {
        /// <summary>
        ///  If an enemy danger rank is estimated to be at this level or higher, put it to sleep if possible.
        /// </summary>
        protected const float PutToSleepDangerThreshold = 3f;
        /// <summary>
        /// How low must the health of an ally or self be to trigger healing.
        /// </summary>
        protected const float HealThreshold = 0.15f;
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void Update()
        {
            base.Update();
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void Start()
        {
            base.Start();
        }
        /// <summary>
        /// Called when an action is requested from the AI.
        /// Highest priority is healing allies. If that is not possible, check if there is a dangerous enough enemy to warrant being put to sleep.
        /// If not, go to the behavior of the base class.
        /// </summary>
        /// <returns> True if some action was executed, otherwise false.</returns>
        protected override bool TryDoAction()
        {
            // Higher priority is healing almost dead characters.
            var allies = CombatantsManager.GetPlayerCharacters(onlyAlive: true);
            if (allies.Any(ally => ally.HitPoints < ally.MaxHitpoints * HealThreshold))
            { 
                if (TryHealAllies())
                {
                    return true;
                }
            }
            // Not healing or not able to heal. Check if there is a monster that should be put to sleep.
            if (TryPutToSleep()) 
            {
                return true;
            }
            // No skills to be used, to standard hero stuff.
            return base.TryDoAction();
        }
        /// <summary>
        /// If knight or ranger is hurt, heal them. Knight takes priority.
        /// If neither is dead, activate healing aura.
        /// While healing aura is active, try to stay close to the more hurt ally.
        /// </summary>
        /// <returns>True if some action was taken, otherwise false.</returns>
        protected bool TryHealAllies()
        {
            // We heal our allies
            if (TryDoMajorHealIfNecessary(Knight) || TryDoMajorHealIfNecessary(Ranger))
            {
                return true;
            }
            // Major heal is not necessary, maybe the cleric is hurting. Activate the healing aura if not activated, if activated, try to move to the more hurt ally.
            if (TryUseSkill(Cleric, Cleric.SelfTargetSkill))
            {
                return true;
            }
            if (Cleric.SelfTargetSkill.IsBeingUsed())
            {
                // Bit hacky - for down target, we set their damage, as 1, i.e. full health, so the other character has always priority if alive.
                var knightDamagePercentage = Knight.IsDown ? 1 : Knight.HitPoints / Knight.MaxHitpoints;
                var rangerDamagePercentage = Ranger.IsDown ? 1 : Ranger.HitPoints / Ranger.MaxHitpoints;
                var auraTarget = rangerDamagePercentage < knightDamagePercentage ? Ranger : Knight;
                var healAuraRange = ((HealingAura)Cleric.SelfTargetSkill).AuraRange;
                if (!auraTarget.IsDown && Vector2.Distance(Cleric.transform.position, auraTarget.transform.position) > healAuraRange)
                {
                    Cleric.GetComponent<MovementController>().MoveToPosition(auraTarget.transform.position);
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Casts heal on the target if possible and necessary, i.e. the target is hurt.
        /// </summary>
        /// <param name="target">The target if the heal spell.</param>
        /// <returns>True if the heal was cast, otherwise false.</returns>
        protected bool TryDoMajorHealIfNecessary(CombatantBase target)
        {
            // We heal allies who are sufficiently hurt, yet also only those who still have enough max hp left for the heal to be worth it.
            return !target.IsDown && target.HitPoints < target.MaxHitpoints * HealThreshold && target.MaxHitpoints / target.TotalMaxHitpoints >= 0.5 && TryUseSkill(target, Cleric.FriendlyTargetSkill);
        }
        /// <summary>
        /// Find the most dangerous enemy. If he is more dangerous than some treshold, casts sleep on him if possible.
        /// </summary>
        /// <returns> True if sleep was used, otherwise false.</returns>
        protected bool TryPutToSleep()
        {
            // Healing not necessary, so instead, let's try to put the most powrful target to sleep if he is powerful enough to warant this.
            var mostPowerfulTarget = GetMostDangerousTarget(dangerousnessThreshold: PutToSleepDangerThreshold);
            return mostPowerfulTarget != null && TryUseSkill(mostPowerfulTarget, Cleric.EnemyTargetSkill);
            
        }
    }
}
