using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class FloatingTextController : MonoBehaviour
    {
        public GameObject TextTemplate;
        public GameObject DialogText;

        double? hideDialogInSeconds;
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

        public void ShowDamageIndicator(int damage)
        {
            ShowDisappearingText("-" + damage.ToString(), Color.red);
        }

        public void ShowHealingIndicator(int healAmount)
        {
            ShowDisappearingText("+ " + healAmount.ToString(), Color.green);
        }

        public void ShowTextLine(String text, Color textColor, double? secondsToBeShown = 2)
        {
            var textComponent = DialogText.GetComponentInChildren<Text>();
            textComponent.text = text;
            textComponent.color = textColor;
            hideDialogInSeconds = secondsToBeShown;
            DialogText.SetActive(true);
        }

        void ShowDisappearingText(String text, Color textColor)
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
