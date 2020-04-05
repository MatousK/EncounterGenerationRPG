using System.Linq;
using Assets.Scripts.Combat;
using Assets.Scripts.Combat.Skills.Character.Cleric;
using Assets.Scripts.Movement;
using UnityEngine;

namespace Assets.Scripts.AI.HeroAI
{
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
        protected override void Update()
        {
            base.Update();
        }

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void OnActionRequired()
        {
            // Higher priority is healing almost dead characters.
            var allies = CombatantsManager.GetPlayerCharacters(onlyAlive: true);
            if (allies.Any(ally => ally.HitPoints < ally.MaxHitpoints * HealThreshold))
            { 
                if (TryHealAllies())
                {
                    return;
                }
            }
            // Not healing or not able to heal. Check if there is a monster that should be put to sleep.
            if (TryPutToSleep()) 
            {
                return;
            }
            // No skills to be used, to standard hero stuff.
            base.OnActionRequired();
        }

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

        protected bool TryDoMajorHealIfNecessary(CombatantBase target)
        {
            // We heal allies who are sufficiently hurt, yet also only those who still have enough max hp left for the heal to be worth it.
            return !target.IsDown && target.HitPoints < target.MaxHitpoints * HealThreshold && target.MaxHitpoints / target.TotalMaxHitpoints >= 0.5 && TryUseSkill(target, Cleric.FriendlyTargetSkill);
        }

        protected bool TryPutToSleep()
        {
            // Healing not necessary, so instead, let's try to put the most powrful target to sleep if he is powerful enough to warant this.
            var mostPowerfulTarget = GetMostDangerousTarget(dangerousnessThreshold: PutToSleepDangerThreshold);
            return mostPowerfulTarget != null && TryUseSkill(mostPowerfulTarget, Cleric.EnemyTargetSkill);
            
        }
    }
}
