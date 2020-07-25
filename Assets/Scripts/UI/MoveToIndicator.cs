using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// An indicator that will appear when the user gives the order to move to an empty state.
    /// Is like a pulsing circle. Starting small, growing bigger, getting small again.
    /// </summary>
    class MoveToIndicator: MonoBehaviour
    {
        /// <summary>
        /// How long should one repetition of the indicator animation last.
        /// </summary>
        public float AnimationDuration = 0.5f;
        /// <summary>
        /// How many repetitions should be done on one move to command.
        /// </summary>
        public float Repetitions = 1;
        /// <summary>
        /// When did this animation start.
        /// </summary>
        float animationStart;
        /// <summary>
        /// Called before the first update. Starts the animation.
        /// </summary>
        void Start()
        {
            transform.localScale = new Vector3(0, 0, 1);
            animationStart = Time.unscaledTime;
        }
        /// <summary>
        /// Called every frame. Calculates how large should the indicator be right now and updates its size.
        /// </summary>
        void Update()
        {
            float timeElapsed = Time.unscaledTime - animationStart;
            if (timeElapsed >= AnimationDuration * Repetitions)
            {
                Destroy(gameObject);
                return;
            }
            // Each repetition is circle expand and contract. Each expansion or contraction is called a phase here.
            // So phase lasts AnimationDuration / 2 seconds.
            float phaseDuration = AnimationDuration / 2;
            var currentPhase = (int)(timeElapsed / phaseDuration);
            var isExpanding = currentPhase % 2 == 0;
            var currentPhaseProgress = (timeElapsed - currentPhase * phaseDuration) / phaseDuration;
            var currentScale = isExpanding ? currentPhaseProgress : 1 - currentPhaseProgress;
            transform.localScale = new Vector3(currentScale, currentScale, 1);
        }
    }
}
