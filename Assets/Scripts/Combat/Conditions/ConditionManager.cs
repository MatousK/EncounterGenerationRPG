using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Combat.Conditions
{
    /// <summary>
    /// Component on each Combatant. This component is used to add and remove conditions from a character.
    /// It also removes the component objects from the combatant once the condition ends.
    /// </summary>
    class ConditionManager: MonoBehaviour
    {
        /// <summary>
        /// List of conditions currently affecting the combatant.
        /// </summary>
        public List<ConditionBase> ActiveConditions = new List<ConditionBase>();
        /// <summary>
        /// Adds the specified condition to the combatant.
        /// If a condition of this type already exists, remove the old one and set this new one.
        /// </summary>
        /// <typeparam name="T">The type of condition to add.</typeparam>
        /// <returns>The added condition. Can be customized to set its attributes.</returns>
        public T AddCondition<T>() where T: ConditionBase
        {
            var conditionToAdd = gameObject.AddComponent<T>();
            // Can't have the same condition multiple times, remove existing conditions.
            for (int i = ActiveConditions.Count - 1; i >= 0; i--)
            {
                if (ActiveConditions[i] is T)
                {
                    RemoveCondition(ActiveConditions[i]);
                }
            }
            ActiveConditions.Add(conditionToAdd);
            conditionToAdd.ConditionEnded += OnConditionEnded;
            return conditionToAdd;
        }
        /// <summary>
        /// Checks whether the combatant has the specified condition.
        /// </summary>
        /// <typeparam name="T">The type of condition to check.</typeparam>
        /// <returns>True if the combatant has the condition, otherwise false.</returns>
        public bool HasCondition<T>() where T: ConditionBase
        {
            return GetComponent<T>() != null;
        }
        /// <summary>
        /// Removes the specified condition from the combatant.
        /// </summary>
        /// <param name="conditionToRemove">The condition that should be removed and destroyed.</param>
        public void RemoveCondition(ConditionBase conditionToRemove)
        {
            ActiveConditions.Remove(conditionToRemove);
            Destroy(conditionToRemove);
        }
        /// <summary>
        /// Called when the condition;s duration is over. Destroys the condition.
        /// </summary>
        /// <param name="sender">The condition that ended.</param>
        /// <param name="e">Unused.</param>
        private void OnConditionEnded(object sender, EventArgs e)
        {
            var endedCondition = sender as ConditionBase;
            RemoveCondition(endedCondition);
        }
    }
}