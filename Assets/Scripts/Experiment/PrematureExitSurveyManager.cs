using Assets.Scripts.Analytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Experiment
{
    /// <summary>
    /// This component will binds itself to application exit and will show an exit survey if the game is not exited properly.
    /// This component should only exist on dungeon screens. Otherwise the survey would appear when exiting the credits.
    /// </summary>
    public class PrematureExitSurveyManager : MonoBehaviour
    {
        /// <summary>
        /// For each experiment group defines which survey should be shown when the user exits the experiment prematurely.
        /// Cannot be a dictionary, as those cannot be shown in the editor.
        /// </summary>
        public List<SurveyLinkDefinition> SurveyUrls;

        /// <summary>
        /// Called before the first update, bind itself to application exit.
        /// </summary>
        private void Start()
        {
            Application.wantsToQuit += Application_wantsToQuit;
        }
        /// <summary>
        /// Called when destroyed, unbinds itself from application exit.
        /// </summary>
        private void OnDestroy()
        {
            Application.wantsToQuit -= Application_wantsToQuit;
        }
        /// <summary>
        /// When the application is exiting, show the survey.
        /// The survey should not be shown if GDPR agreement was revoked.
        /// </summary>
        /// <returns>Always true, we do not want to interrupt the quitting.</returns>
        private bool Application_wantsToQuit()
        {
            // If the agreement is not revoked, show the survey;
            var analyticsService = FindObjectOfType<AnalyticsService>();
            if (analyticsService != null && !analyticsService.DidFailToSendRevokeAgreement && !analyticsService.RevokedAgreement)
            {
                var url = GetExitSurveyUrl();
                if (url != null)
                {
                    Application.OpenURL(url);
                }
            }
            return true;
        }
        /// <summary>
        /// Get a survey for the url for the current experiment group.
        /// </summary>
        /// <returns>The url to be used.</returns>
        private string GetExitSurveyUrl()
        {
            var abTestingService = FindObjectOfType<AbTestingManager>();
            if (abTestingService == null)
            {
                return null;
            }
            var linkDefinition = SurveyUrls.Single(surveyUrlDefinition => surveyUrlDefinition.ExperimentGroup == abTestingService.CurrentExperimentGroup);
            return linkDefinition?.SurveyUrl;
        }
    }
    /// <summary>
    /// Specifies that an experiment group should have some survey URL.
    /// </summary>
    [Serializable]
    public class SurveyLinkDefinition
    {
        /// <summary>
        /// Experiment group with assigned URL.
        /// </summary>
        public ExperimentGroup ExperimentGroup;
        /// <summary>
        /// The URL assigned to the group.
        /// </summary>
        public string SurveyUrl;
    }
}
