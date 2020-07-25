using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.GameFlow;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.UI.MainMenu
{
    /// <summary>
    /// Component which provides the functionality to game menu.
    /// </summary>
    class MainMenuController : MonoBehaviour
    {
        /// <summary>
        /// Knows about which levels to load and in what order.
        /// </summary>
        private LevelLoader levelLoader;
        /// <summary>
        /// Called before first update. Find references to dependencies.
        /// </summary>
        private void Start()
        {
            levelLoader = FindObjectsOfType<LevelLoader>().FirstOrDefault(loader => !loader.IsPendingKill);
        }
        /// <summary>
        /// Called when the user accepts the privacy agreement. Playes the animation which shows the menu.
        /// </summary>
        public void OnPrivacyAgreementClicked()
        {
            GetComponent<Animation>().Play("HidePrivacyShowMenu");
        }
        /// <summary>
        /// Starts story mode, i.e. new experiment.
        /// </summary>
        public void StartStoryMode()
        {
            levelLoader.StartStoryMode();
        }
        /// <summary>
        /// Start free mode. Currently not used.
        /// </summary>
        public void StartFreeMode()
        {
            levelLoader.StartFreeMode();
        }
        /// <summary>
        /// Opens the Credits scene.
        /// </summary>
        public void ShowCredits()
        {
            levelLoader.OpenCredits();
        }
        /// <summary>
        /// Exits the came.
        /// </summary>
        public void Exit()
        {
            Application.Quit();
        }
    }
}
