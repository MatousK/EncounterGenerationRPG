using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.GameFlow;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class TypewriterWithSurveyScreen: MonoBehaviour
    {
        [HideInInspector]
        public String SurveyLink;
        public TypewriterText IntroTypewriterText;
        public LevelDefinition LevelDefinition;
        public Button SurveyButton;
        public Button ContinueButton;
        public GameObject SurveyButtonsContainer;
        public void Start()
        {
            IntroTypewriterText.TextAnimationDone += IntroTypewriterText_TextAnimationDone;
            IntroTypewriterText.TextToDisplay = LevelDefinition.IntroTexts?.FirstOrDefault() ?? "";
            var hasSurvey = !string.IsNullOrEmpty(SurveyLink);
            ContinueButton.interactable = !hasSurvey;
            if (!hasSurvey)
            {
                Destroy(SurveyButton.gameObject);
            }
        }

        private void IntroTypewriterText_TextAnimationDone(object sender, EventArgs e)
        {
            SurveyButtonsContainer.SetActive(true);
        }

        public void ContinuePressed()
        {
            FindObjectOfType<LevelLoader>().LoadLevel(LevelDefinition);
        }

        public void SurveyPressed()
        {
            Application.OpenURL(SurveyLink);
            ContinueButton.interactable = true;
        }
    }
}
