using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts.Combat
{
    /// <summary>
    /// When a player uses a skill on someone or attacks an enemy, a blinking indicator will appear under him to notify the player that yes, a command was given.
    /// This class controls that indicator. Call <see cref="DisplayConfirmation"/> to start blinking.
    /// </summary>
    [ExecuteAfter(typeof(SelectableObject))]
    public class CommandConfirmationIndicator: MonoBehaviour
    {
        /// <summary>
        /// How long should the indicator remain visible during a blink and what should be the delay between those blinks.
        /// This is a duration of the entire blink animation, so circle will be visible for half of this value and than half of this value will be the delay before the circle appears again.
        /// </summary>
        public float ConfirmationBlinkLength = 0.3f;
        /// <summary>
        /// How many times should the circle blink to show the confirmation.
        /// </summary>
        public int ConfirmationBlinkCount = 3;
        /// <summary>
        /// Time when the current command confirmation started. Null if no command confirmation is happening.
        /// </summary>
        private float? confirmationStart;
        /// <summary>
        /// Circle that is actually drawn to indicate the command confirmation.
        /// </summary>
        private Circle indicatorCircle;
        /// <summary>
        /// If true, the circle will start invisible when confirmation starts. So <see cref="ConfirmationBlinkLength"/>/2 seconds will elapse before a circle appears.
        /// If false, the circle will appear immediately when <see cref="DisplayConfirmation"/> is called.
        /// </summary>
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
        /// <summary>
        /// Start the blinking animation to confirm a command.
        /// </summary>
        public void DisplayConfirmation()
        {
            confirmationStart = Time.unscaledTime;
            startIndicatorOff = indicatorCircle.IsVisible;
        }
    }
}
