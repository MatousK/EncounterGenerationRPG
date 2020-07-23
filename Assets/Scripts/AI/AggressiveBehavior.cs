using Assets.Scripts.Combat;
using Assets.Scripts.Combat.Conditions;
using Assets.Scripts.Cutscenes;
using UnityEngine;

namespace Assets.Scripts.AI
{
    /// <summary>
    /// This component can be added to a combatant object. It will attack its target with basic attacks if it has nothing better to do.
    /// If no target is set, attack the closest enemy that is not sleeping.
    /// Used by heroes in the actual game so they do not just stand around doing nothing.
    /// </summary>
    public class AggressiveBehavior : MonoBehaviour
    {
        /// <summary>
        /// This component has information about all combatants in the game.
        /// </summary>
        CombatantsManager combatantsManager;
        /// <summary>
        /// The component that is responsible for using basic attacks on the target over and over again.
        /// </summary>
        AutoAttacking autoAttacking;
        /// <summary>
        /// The combatant this AI controls.
        /// </summary>
        CombatantBase selfCombatant;
        /// <summary>
        /// Component that knows whether the cutscene is active. Necessary because an active cutscene blocks AI.
        /// </summary>
        CutsceneManager cutsceneManager;

        private void Start()
        {
            combatantsManager = FindObjectOfType<CombatantsManager>();
            autoAttacking = GetComponent<AutoAttacking>();
            selfCombatant = GetComponent<CombatantBase>();
            cutsceneManager = FindObjectOfType<CutsceneManager>();
        }
        // Update is called once per frame
        void Update()
        {
            if (cutsceneManager.IsCutsceneActive)
            {
                autoAttacking.Target = null;
                return;
            }
            if (autoAttacking.Target == null && !selfCombatant.IsDoingNonAutoAttackAction())
            {
                AttackClosestOpponent();
            }
        }
        /// <summary>
        /// Attacks the nearest opponent who is not asleep.
        /// </summary>
        void AttackClosestOpponent()
        {
            var collider = GetComponent<Collider2D>();
            float closestDistance = float.PositiveInfinity;
            CombatantBase closestTarget = null;
            foreach (var opponent in combatantsManager.GetOpponentsFor(selfCombatant, onlyAlive: true))
            {
                if (opponent.GetComponent<ConditionManager>().HasCondition<SleepCondition>())
                {
                    // Do not auto attack sleeping opponents.
                    continue;
                }
                var distanceToOpponent = opponent.GetComponent<Collider2D>().Distance(collider).distance;
                if (distanceToOpponent < closestDistance)
                {
                    closestDistance = distanceToOpponent;
                    closestTarget = opponent;
                }
            }
            autoAttacking.Target = closestTarget;
        }
    }
}
