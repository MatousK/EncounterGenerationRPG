using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Combat.Conditions
{
    class ConditionManager: MonoBehaviour
    {
        public List<ConditionBase> ActiveConditions = new List<ConditionBase>();

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

        public bool HasCondition<T>() where T: ConditionBase
        {
            return GetComponent<T>() != null;
        }

        public void RemoveCondition(ConditionBase conditionToRemove)
        {
            ActiveConditions.Remove(conditionToRemove);
            Destroy(conditionToRemove);
        }

        private void OnConditionEnded(object sender, EventArgs e)
        {
            var endedCondition = sender as ConditionBase;
            RemoveCondition(endedCondition);
        }
    }
}