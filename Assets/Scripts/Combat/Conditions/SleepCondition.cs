using UnityEngine;

namespace Assets.Scripts.Combat.Conditions
{
    /// <summary>
    /// When added to a creature, will put it to sleep for a specified duration, then waking it up.
    /// If it takes damage, it is woken up immediately.
    /// Asleep characters cannot take any actions (which is controlled by the parent class, <see cref="StunCondition"/>)
    /// </summary>
    public class SleepCondition: StunCondition
    {
        /// <summary>
        /// Hitpoints when this condition was applied. If they are reduced, the creature is woken up.
        /// </summary>
        private float originalHitpoints;
        /// <summary>
        /// Store the HP at the start of this condition.
        /// </summary>
        protected override void Start()
        {
            originalHitpoints = GetComponent<CombatantBase>().HitPoints;
            base.Start();
        }
        /// <summary>
        /// Check if hit points changed. If they did, wake up the character.
        /// </summary>
        protected override void Update()
        {
            if (originalHitpoints != GetComponent<CombatantBase>().HitPoints)
            {
                // Took damage, wake up.
                EndCondition();
                return;
            }
            base.Update();
        }
        /// <summary>
        /// Condition changes the animation state so the monster looks asleep.
        /// AI will check for this condition to ensure it does not do anything.
        /// </summary>
        protected override void StartCondition()
        {
            base.StartCondition();
            GetComponent<Animator>().SetBool("Asleep", true);
        }
        /// <summary>
        /// Reset the animation state, waking the creature up. AI will start giving orders.
        /// </summary>
        protected override void EndCondition()
        {
            base.EndCondition();
            GetComponent<Animator>().SetBool("Asleep", false);
        }
    }
}