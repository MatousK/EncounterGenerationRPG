using Assets.Scripts.Analytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    class RevokeActivityIndicator : MonoBehaviour
    {
        public Text ProgressIndicator;
        public Text AttemptCounter;
        public Text ResultText;
        public float DelayBeforeShutdownAfterSuccess = 5;
        private float? timedShutDownStart;

        AnalyticsService analyticsService;
        private void Start()
        {
            analyticsService = FindObjectOfType<AnalyticsService>();
        }
        private void Update()
        {
            int attemptCounterDots = ((int)(Time.realtimeSinceStartup * 2)) % 5;
            string dots = "";
            for (int i = 0; i < attemptCounterDots; ++i)
            {
                dots += ".";
            }
            ProgressIndicator.text = dots;
            if (analyticsService.RevokeAttemptIndex != 0)
            {
                AttemptCounter.gameObject.SetActive(true);
                AttemptCounter.text = $"There are problems communicating with the server \n Current attempt: {analyticsService.RevokeAttemptIndex}";
            }
            if (analyticsService.DidFailToSendRevokeAgreement)
            {
                ProgressIndicator.text = "";
                ResultText.text = "There were too many unsuccessful attempts.\n" +
                                "Please send the request manually to the following email:\n" +
                                "mattkozma @hotmail.com\n" +
                                "Be sure to include your user ID:\n" +
                                analyticsService.UserGuid.ToString() + "\n" +
                                "You can exit by pressing ALT-F4";
                ResultText.gameObject.SetActive(true);
            }
            if (analyticsService.RevokedAgreement)
            {
                ProgressIndicator.text = "";
                ResultText.gameObject.SetActive(true);
                ResultText.text = " Done! The game will quit shortly.";
                if (timedShutDownStart == null)
                {
                    timedShutDownStart = Time.unscaledTime;
                } 
                else if (Time.unscaledTime - timedShutDownStart.Value > DelayBeforeShutdownAfterSuccess)
                {
                    Application.Quit();
                }
            }
        }
    }
}
