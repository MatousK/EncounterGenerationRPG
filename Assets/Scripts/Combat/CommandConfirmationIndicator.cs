using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts.Combat
{
    [ExecuteAfter(typeof(SelectableObject))]
    public class CommandConfirmationIndicator: MonoBehaviour
    {
        public float ConfirmationBlinkLength = 0.3f;
        public int ConfirmationBlinkCount = 3;
        private float? confirmationStart;
        private Circle indicatorCircle;
        private bool startIndicatorOff;

        private void Awake()
        {
            indicatorCircle = GetComponent<Circle>();
        }

        private void Update()
        {
            if (confirmationStart == null)
            {
                return;
            }
            // This is a blinking indicator - for some time it should be on, then off, then on, then off, etc.
            // Blink is going out and on, one phase is going on or off, so divide by 2 to get a phase length;
            var confirmationPhaseLength = ConfirmationBlinkLength / 2;
            var confirmationAnimationTime = Time.unscaledTime - confirmationStart.Value;
            var currentPhase = (int)(confirmationAnimationTime / confirmationPhaseLength);
            if (currentPhase >= ConfirmationBlinkCount * 2)
            {
                confirmationStart = null;
                return;
            }

            indicatorCircle.IsVisible = (currentPhase % 2 == 0) != startIndicatorOff;
        }

        public void DisplayConfirmation()
        {
            confirmationStart = Time.unscaledTime;
            startIndicatorOff = indicatorCircle.IsVisible;
        }
    }
}
