using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.UI
{
    class MoveToIndicator: MonoBehaviour
    {
        public float AnimationDuration = 0.5f;
        public float Repetitions = 1;

        float animationStart;

        void Start()
        {
            transform.localScale = new Vector3(0, 0, 1);
            animationStart = Time.unscaledTime;
        }
        void Update()
        {
            float timeElapsed = Time.unscaledTime - animationStart;
            if (timeElapsed >= AnimationDuration * Repetitions)
            {
                Destroy(gameObject);
                return;
            }
            // Each repetition is circle expand and contract. Each expansion or contraction is called a phase here.
            // So phase lasts AnimationDuration / seconds.
            float phaseDuration = AnimationDuration / 2;
            var currentPhase = (int)(timeElapsed / phaseDuration);
            var isExpanding = currentPhase % 2 == 0;
            var currentPhaseProgress = (timeElapsed - currentPhase * phaseDuration) / phaseDuration;
            var currentScale = isExpanding ? currentPhaseProgress : 1 - currentPhaseProgress;
            transform.localScale = new Vector3(currentScale, currentScale, 1);
        }
    }
}
