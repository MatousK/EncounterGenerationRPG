using Assets.Scripts.UI.Credits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// The indicator that allows the player to skip a typewriter text or credits.
    /// Skips on enter.
    /// Is invisible initially, is displayed when the presses anything else.
    /// </summary>
    public class SkipButton: MonoBehaviour
    {
        /// <summary>
        /// If true, we are skipping credits.
        /// </summary>
        public bool isCredits;
        /// <summary>
        /// The typewriter text to skip.
        /// </summary>
        private TypewriterText controlledTypewriterText;
        /// <summary>
        /// Called before first update.
        /// If not in credits. Tries to find the typewriter text we should skip and binds to its done event, as once the text is done, the skip prompt should stop blinking.
        /// </summary>
        private void Start()
        {
            if (!isCredits)
            {
                controlledTypewriterText = FindObjectOfType<TypewriterText>();
                controlledTypewriterText.TextAnimationDone += ControlledTypewriterText_TextAnimationDone;
            }
        }

        /// <summary>
        /// Called every frame to detect input. On Return skips the cutscene or typewriter text. On anything else, shows the indicator.
        /// </summary>
        private void Update()
        {
            if (UnityEngine.Input.GetKeyUp(KeyCode.Return))
            {
                if (isCredits)
                {
                    FindObjectOfType<CreditsController>().CreditsOver();
                } else
                {
                    controlledTypewriterText.FinishAnimation();
                }
            }
            else if (UnityEngine.Input.anyKeyDown)
            {
                GetComponent<Animation>().Play();
            }
        }
        /// <summary>
        /// Called when typewriter finishes. Destroys this object as skipping a finished animation makes little sense.
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Arguments for the event.</param>
        private void ControlledTypewriterText_TextAnimationDone(object sender, EventArgs e)
        {
            Destroy(gameObject);
        }
    }
}
