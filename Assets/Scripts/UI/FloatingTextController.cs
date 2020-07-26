using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// Component which should be available to every combatant. When he takes damage or is healed, an indicator is shown above him to show how much was he healed or damage.
    /// This class spawns that indicator.
    /// It also supports floating texts above characters as a line they say, by this is never used in the game.
    /// </summary>
    public class FloatingTextController : MonoBehaviour
    {
        /// <summary>
        /// Template for the healing or damage indicator.
        /// </summary>
        public GameObject TextTemplate;
        /// <summary>
        /// The dialog a character says that will stay there for some amount of seconds.
        /// </summary>
        public GameObject DialogText;
        /// <summary>
        /// When a dialog is shown, this is set to the remaining duration this dialog should remain visible.
        /// Updated every frame.
        /// Once reaches zero, the dialog is destroyed.
        /// </summary>
        double? hideDialogInSeconds;
        /// <summary>
        /// Called every frame. Updates the <see cref="hideDialogInSeconds"/> and if elapsed, hides the dialog.
        /// </summary>
        private void Update()
        {
            if (hideDialogInSeconds == null)
            {
                return;
            }
            hideDialogInSeconds = hideDialogInSeconds.Value - Time.deltaTime;
            if (hideDialogInSeconds <= 0)
            {
                DialogText.SetActive(false);
                hideDialogInSeconds = null;
            }
        }
        /// <summary>
        /// Shows a damage indicator showing the character took some damage.
        /// </summary>
        /// <param name="damage">How much damage was taken.</param>
        public void ShowDamageIndicator(int damage)
        {
            ShowDisappearingText("-" + damage.ToString(), Color.red);
        }
        /// <summary>
        /// Shows a damage indicator showing the character was healed.
        /// </summary>
        /// <param name="healAmount">How much damage was healed.</param>
        public void ShowHealingIndicator(int healAmount)
        {
            ShowDisappearingText("+ " + healAmount.ToString(), Color.green);
        }
        /// <summary>
        /// Shows a text line above the character which stays there for <paramref name="secondsToBeShown"/> seconds.
        /// </summary>
        /// <param name="text">Text to show.</param>
        /// <param name="textColor">Color of the text to be shown.</param>
        /// <param name="secondsToBeShown">How long should the text stay on screen.</param>
        public void ShowTextLine(string text, Color textColor, double? secondsToBeShown = 2)
        {
            var textComponent = DialogText.GetComponentInChildren<Text>();
            textComponent.text = text;
            textComponent.color = textColor;
            hideDialogInSeconds = secondsToBeShown;
            DialogText.SetActive(true);
        }
        /// <summary>
        /// Show a text that will float above this character and disappear and destroy itself on its own.
        /// </summary>
        /// <param name="text">Text that should be shown.</param>
        /// <param name="textColor">Color of that text.</param>
        void ShowDisappearingText(string text, Color textColor)
        {
            var textToShow = Instantiate(TextTemplate, transform, false);
            var textComponent = textToShow.GetComponentInChildren<Text>();
            textComponent.color = textColor;
            textComponent.text = text;
            textToShow.SetActive(true);
            textToShow.GetComponent<Animator>().SetTrigger("Disappear");
        }
    }
}
