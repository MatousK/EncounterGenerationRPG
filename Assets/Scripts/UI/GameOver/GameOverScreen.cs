using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.GameFlow;
using UnityEngine;

namespace Assets.Scripts.UI.GameOver
{
    /// <summary>
    /// Controls the game over screen.
    /// Shows it when game over happens and hides it on game reload.
    /// </summary>
    class GameOverScreen: MonoBehaviour
    {
        /// <summary>
        /// How long will we wait before showing the game over screen after game over.
        /// </summary>
        public float TimeBeforeGameOver = 2;
        /// <summary>
        /// The game over screen UI to enable when game over happens.
        /// </summary>
        public GameObject GameOverScreenUi;
        /// <summary>
        /// The component which does the fade to white when the player resets chooses to try again.
        /// </summary>
        public TryAgainOverlay TryAgainOverlay;
        /// <summary>
        /// Class which can notify us about game over and the exact time the game gets reloaded.
        /// </summary>
        private GameStateManager gameStateManager;

        /// <summary>
        /// Executed every frame. Ensures we have a valid reference to the <see cref="gameStateManager"/>
        /// </summary>
        private void Update()
        {
            if (gameStateManager == null)
            { 
                gameStateManager = FindObjectOfType<GameStateManager>();
                if (gameStateManager != null)
                {
                    // Bind to the game state manager once it exists.
                    gameStateManager.GameOver += GameStateManager_GameOver;
                    gameStateManager.GameReloaded += GameStateManager_GameReloaded;
                }
            }
        }
        /// <summary>
        /// Unbind from events on destroy.
        /// </summary>
        private void OnDestroy()
        {
            if (gameStateManager != null)
            {
                gameStateManager.GameOver -= GameStateManager_GameOver;
                gameStateManager.GameReloaded -= GameStateManager_GameReloaded;
            }
        }
        /// <summary>
        /// When game is reloaded, hide the game over overlay.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Arguments of the event.</param>
        private void GameStateManager_GameReloaded(object sender, EventArgs e)
        {
            GameOverScreenUi.SetActive(false);
        }
        /// <summary>
        /// On game over, show the game over screen after the delay <see cref="TimeBeforeGameOver"/>
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Arguments of the event.</param>
        private void GameStateManager_GameOver(object sender, EventArgs e)
        {
            StartCoroutine(StartGameOverAnimation());
        }
        /// <summary>
        /// Wait for <see cref="TimeBeforeGameOver"/> seconds before showing the game over screen UI and playing the appear animation.
        /// </summary>
        /// <returns></returns>
        private IEnumerator StartGameOverAnimation()
        {
            yield return new WaitForSecondsRealtime(TimeBeforeGameOver);
            GameOverScreenUi.SetActive(true);
            GameOverScreenUi.GetComponent<Animation>().Play();
        }
        /// <summary>
        /// Called when the player chooses to try again.
        /// </summary>
        public void OnTryAgain()
        {
            TryAgainOverlay.StartTryAgainAnimation();
        }
        /// <summary>
        /// Called when the player chooses to quit the experiment.
        /// </summary>
        public void OnGiveUp()
        {
            Application.Quit();
        }
    }
}
