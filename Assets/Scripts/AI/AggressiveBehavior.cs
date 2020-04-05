using Assets.Scripts.Combat;
using Assets.Scripts.Combat.Conditions;
using Assets.Scripts.Cutscenes;
using UnityEngine;

namespace Assets.Scripts.AI
{
    public class AggressiveBehavior : MonoBehaviour
    {
        CombatantsManager combatantsManager;
        AutoAttacking autoAttacking;
        CombatantBase selfCombatant;
        CutsceneManager cutsceneManager;

        private void Awake()
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
