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
    /// Will binds itself to application exit and will show an exit survey if the game is not exited properly.
    /// </summary>
    public class PrematureExitSurveyManager : MonoBehaviour
    {
        public List<SurveyLinkDefinition> SurveyUrls;

        private void Start()
        {
            Application.wantsToQuit += Application_wantsToQuit;
        }

        private void OnDestroy()
        {
            Application.wantsToQuit -= Application_wantsToQuit;
        }

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

    [Serializable]
    public class SurveyLinkDefinition
    {
        public ExperimentGroup ExperimentGroup;
        public string SurveyUrl;
    }
}
