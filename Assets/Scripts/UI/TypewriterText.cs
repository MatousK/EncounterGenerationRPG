using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class TypewriterText: MonoBehaviour
    {
        public event EventHandler TextAnimationDone;
        public float Speed = 0.1f;

        private string textToDisplay;
        private Text controlledText;
        private float startTime;
        private bool isAnimationFinished;

        public void Start()
        {
            startTime = Time.time;
            controlledText = GetComponent<Text>();
            textToDisplay = controlledText.text;
            controlledText.text = "";
        }

        public void Update()
        {
            if (isAnimationFinished)
            {
                return;
            }

            var elapsedTime = Time.time - startTime;
            var shownCharacters = (int)(elapsedTime / Speed);
            shownCharacters = Math.Min(shownCharacters, textToDisplay.Length);
            controlledText.text = textToDisplay.Substring(0, shownCharacters);
            if (shownCharacters == textToDisplay.Length)
            {
                FinishAnimation();
            }
        }

        public void FinishAnimation()
        {
            controlledText.text = textToDisplay;
            isAnimationFinished = true;
            TextAnimationDone?.Invoke(this, new EventArgs());
        }
    }
}
