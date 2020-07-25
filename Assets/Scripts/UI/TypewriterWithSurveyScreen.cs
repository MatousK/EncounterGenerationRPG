using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Analytics;
using Assets.Scripts.GameFlow;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// The component controlling the screen with the typewriter and surveys.
    /// Waits for the typewriter to finish.
    /// Once done, shows the buttons to show survey and/or go th next level.
    /// </summary>
    public class TypewriterWithSurveyScreen: MonoBehaviour
    {
        /// <summary>
        /// Link to the survey the user should fill out. If null, no survey should be filled out.
        /// </summary>
        [HideInInspector]
        public string SurveyLink;
        /// <summary>
        /// The component that can show the typewriter text.
        /// </summary>
        public TypewriterText IntroTypewriterText;
        /// <summary>
        /// The level for which we should show the typewriter text and/or surveys
        /// </summary>
        public LevelDefinition LevelDefinition;
        /// <summary>
        /// The button that sends the user to the survey.
        /// </summary>
        public Button SurveyButton;
        /// <summary>
        /// The button that lets the user continue to the next level.
        /// </summary>
        public Button ContinueButton;
        /// <summary>
        /// The button which copies the user ID to clipboard.
        /// </summary>
        public Button CopyUserIDButton;
        /// <summary>
        /// The container which has all the buttons this component controls.
        /// </summary>
        public GameObject SurveyButtonsContainer;
        /// <summary>
        /// Called before first frame. Starts the typewriter effect.
        /// If a survey is not provided, destroy survey related buttons.
        /// </summary>
        public void Start()
        {
            IntroTypewriterText.TextAnimationDone += IntroTypewriterText_TextAnimationDone;
            IntroTypewriterText.TextToDisplay = LevelDefinition.IntroTexts?.FirstOrDefault() ?? "";
            var hasSurvey = !string.IsNullOrEmpty(SurveyLink);
            ContinueButton.interactable = !hasSurvey;
            if (!hasSurvey)
            {
                Destroy(SurveyButton.gameObject);
                Destroy(CopyUserIDButton.gameObject);
            }
        }
        /// <summary>
        /// Once the typewriter effect is done, show the buttons.
        /// </summary>
        /// <param name="sender">Sender of this event.</param>
        /// <param name="e">Arguments of the event.</param>
        private void IntroTypewriterText_TextAnimationDone(object sender, EventArgs e)
        {
            SurveyButtonsContainer.SetActive(true);
        }
        /// <summary>
        /// User pressed the <see cref="CopyUserIDButton"/>.
        /// Copies the user ID to clipboard.
        /// Also enables the <see cref="SurveyButton"/> which is disabled until GUID is copied.
        /// </summary>
        public void CopyUserId()
        {
            var analyticsService = FindObjectOfType<AnalyticsService>();
            var guid = analyticsService.UserGuid;
            GUIUtility.systemCopyBuffer = guid.ToString();
            // Enable launching of the survey.
            SurveyButton.interactable = true;
        }
        /// <summary>
        /// User pressed the <see cref="ContinueButton"/>. 
        /// Load the next level.
        /// </summary>
        public void ContinuePressed()
        {
            FindObjectOfType<LevelLoader>().LoadLevel(LevelDefinition);
        }
        /// <summary>
        /// User pressed the <see cref="SurveyButton"/>
        /// Shows the survey and enables the <see cref="ContinueButton"/>
        /// </summary>
        public void SurveyPressed()
        {
            Application.OpenURL(SurveyLink);
            ContinueButton.interactable = true;
        }
    }
}
