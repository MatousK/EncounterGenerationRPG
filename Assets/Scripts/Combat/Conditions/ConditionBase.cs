using System;
using UnityEngine;

namespace Assets.Scripts.Combat.Conditions
{
    /// <summary>
    /// Base class for all conditions in the game. In RPGs these are usually things like bleeding, sleeping, burning etc.
    /// Can be put on a character to somehow affect it, eithe positively or negatively. Has set duration.
    /// </summary>
    public abstract class ConditionBase : MonoBehaviour
    {
        /// <summary>
        /// How much longer should the condition remain active.
        /// </summary>
        public float RemainingDuration = float.PositiveInfinity;
        /// <summary>
        /// Event raised when the condition's duration has ended or it was otherwise removed.
        /// </summary>
        public event EventHandler<EventArgs> ConditionEnded;
        /// <summary>
        /// This ConditionBase component is created when applied, so on Start() we should apply all the condition effects. <see cref="StartCondition"/>
        /// </summary>
        protected virtual void Start()
        {
            StartCondition();
        }
        /// <summary>
        /// Updates the time remaining. If the condition is over, end its effects. <see cref="EndCondition"/>
        /// </summary>
        protected virtual void Update()
        {
            RemainingDuration -= Time.deltaTime;
            if (RemainingDuration <= 0)
            {
                EndCondition();
            }
        }
        /// <summary>
        /// Should be overriden by child classes to apply condition effects.
        /// </summary>
        protected virtual void StartCondition() { }
        /// <summary>
        /// Should be overriden by child classes to end condition effects.
        /// </summary>
        protected virtual void EndCondition()
        {
            ConditionEnded?.Invoke(this, new EventArgs()); 
        }
    }
}