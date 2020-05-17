using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Analytics;
using Assets.Scripts.EncounterGenerator;
using Assets.Scripts.Experiment;
using Assets.Scripts.GameFlow;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.UI.Credits
{
    public class CreditsController: MonoBehaviour
    {
        public float scrollSpeed = 10;
        public GameObject ScrollText;
        private double animationStart;
        private void Start()
        {
            animationStart = Time.unscaledTime;
            var canvasTransform = transform as RectTransform;
            var canvasHeight = canvasTransform.sizeDelta.y;
            var scrollTransform = ScrollText.transform as RectTransform;
            scrollTransform.anchoredPosition = new Vector2(scrollTransform.anchoredPosition.x, -canvasHeight);
        }

        private void Update()
        {
            var scrollTransform = ScrollText.transform as RectTransform;
            var canvasHeight = scrollTransform.sizeDelta.y;
            var newY = scrollTransform.anchoredPosition.y + Time.deltaTime * scrollSpeed;
            scrollTransform.anchoredPosition = new Vector2(scrollTransform.anchoredPosition.x, newY);
            if (canvasHeight < newY)
            {
                CreditsOver();
            }
        }

        public void CreditsOver()
        {
            // Reset data - generate new AB testing group, generate new GUID, reset the matrix.
            var analyticsService = FindObjectOfType<AnalyticsService>();
            if (analyticsService != null)
            {
                analyticsService.ResetGuid();
            }
            var difficultyMatrixProvider = FindObjectOfType<DifficultyMatrixProvider>();
            if (difficultyMatrixProvider != null)
            {
                difficultyMatrixProvider.ReloadMatrix(true);
            }
            var abTestingManager = FindObjectOfType<AbTestingManager>();
            if (abTestingManager != null)
            {
                abTestingManager.ResetTestingGroup();
            }

            FindObjectOfType<LevelLoader>().OpenMainMenu();
        }
    }
}
