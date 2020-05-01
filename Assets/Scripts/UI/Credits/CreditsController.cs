using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Analytics;
using Assets.Scripts.GameFlow;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.UI.Credits
{
    public class CreditsController: MonoBehaviour
    {
        public void CreditsOver()
        {
            var analyticsService = FindObjectOfType<AnalyticsService>();
            if (analyticsService != null)
            {
                analyticsService.ResetGuid();
            }

            FindObjectOfType<LevelLoader>().OpenMainMenu();
        }
    }
}
