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
    /// <summary>
    /// An overlay covering the screen showing that revocation of agreement is in progress.
    /// Once revocation is done, exit the game.
    /// </summary>
    class RevokeActivityIndicator : MonoBehaviour
    {
        /// <summary>
        /// UI Text label changing its text to show that yes, we are trying to revoke, we are not stuck.
        /// </summary>
        public Text ProgressIndicator;
        /// <summary>
        /// UI Text label showing how many times did we try to revoke.
        /// </summary>
        public Text AttemptCounter;
        /// <summary>
        /// UI Text label showing the result of the revocation, either success or fail.
        /// </summary>
        public Text ResultText;
        /// <summary>
        /// Upon revocation success, how long will the screen stay on before the game exiting.
        /// </summary>
        public float DelayBeforeShutdownAfterSuccess = 5;
        /// <summary>
        /// When did we start the shut down after delay.
        /// </summary>
        private float? timedShutDownStart;
        /// <summary>
        /// The service responsible for sending analytics information to the server.
        /// </summary>
        AnalyticsService analyticsService;
        /// <summary>
        /// Called before first update. Initializes references to the dependencies.
        /// </summary>
        private void Start()
        {
            analyticsService = FindObjectOfType<AnalyticsService>();
        }
        /// <summary>
        /// Called every frame. Updates the text fields showing how far are we with the revocation.
        /// </summary>
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
