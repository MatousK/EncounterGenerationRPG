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
            var analyticsService = FindObjectOfType<AnalyticsService>();
            if (analyticsService != null)
            {
                analyticsService.ResetGuid();
            }

            FindObjectOfType<LevelLoader>().OpenMainMenu();
        }
    }
}
