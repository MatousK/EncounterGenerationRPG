using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// Component which writes the text to sme text filed character by character.
    /// </summary>
    public class TypewriterText: MonoBehaviour
    {
        /// <summary>
        /// Event that is raised when the animation is done.
        /// </summary>
        public event EventHandler TextAnimationDone;
        /// <summary>
        /// Speed of the animation. This many characters will appear per second.
        /// </summary>
        public float Speed = 0.1f;
        /// <summary>
        /// Text that should be shown character by character.
        /// </summary>
        public string TextToDisplay;
        /// <summary>
        /// UI Text that displays the text to the user.
        /// </summary>
        private Text controlledText;
        /// <summary>
        /// Time when the typewriter animation started.
        /// </summary>
        private float startTime;
        /// <summary>
        /// If true, we have finished the animation.
        /// </summary>
        private bool isAnimationFinished;
        /// <summary>
        /// Called before the first Update. Starts the animation.
        /// </summary>
        public void Start()
        {
            startTime = Time.time;
            controlledText = GetComponent<Text>();
            controlledText.text = "";
        }
        /// <summary>
        /// Called every frame. Updates the characters being shown right now.
        /// </summary>
        public void Update()
        {
            if (isAnimationFinished)
            {
                return;
            }

            var elapsedTime = Time.time - startTime;
            var shownCharacters = (int)(elapsedTime / Speed);
            shownCharacters = Math.Min(shownCharacters, TextToDisplay.Length);
            controlledText.text = TextToDisplay.Substring(0, shownCharacters);
            if (shownCharacters == TextToDisplay.Length)
            {
                FinishAnimation();
            }
        }
        /// <summary>
        /// Called when the animation is finished or skipped. Stops the typewriter effect, sets the entire text and raises the <see cref="TextAnimationDone"/> event.
        /// </summary>
        public void FinishAnimation()
        {
            controlledText.text = TextToDisplay;
            isAnimationFinished = true;
            TextAnimationDone?.Invoke(this, new EventArgs());
        }
    }
}
