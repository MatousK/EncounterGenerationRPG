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
    /// <summary>
    /// Scrolls the credits up until they completely disappears. Then we reset everything about the session and open main menu
    /// </summary>
    public class CreditsController: MonoBehaviour
    {
        /// <summary>
        /// How fast should the credits scroll.
        /// </summary>
        public float scrollSpeed = 10;
        /// <summary>
        /// The object containing the scrolled text.
        /// </summary>
        public GameObject ScrollText;
        /// <summary>
        /// Called before the first update. Teleports the credits so they are just below the screen.
        /// </summary>
        private void Start()
        {
            var canvasTransform = transform as RectTransform;
            var canvasHeight = canvasTransform.sizeDelta.y;
            var scrollTransform = ScrollText.transform as RectTransform;
            scrollTransform.anchoredPosition = new Vector2(scrollTransform.anchoredPosition.x, -canvasHeight);
        }
        /// <summary>
        /// Called every frame. Moves the credits slightly up. If we got out of bounds and the entire credits scrolled, call <see cref="CreditsOver"/>
        /// </summary>
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
        /// <summary>
        /// Call when the credits are over.
        /// Reset everything that should not persist between sessions - experiment group, analytics and matrix - and open main menu.
        /// </summary>
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
