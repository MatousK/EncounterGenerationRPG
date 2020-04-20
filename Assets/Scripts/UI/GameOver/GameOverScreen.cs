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
    class GameOverScreen: MonoBehaviour
    {
        public float TimeBeforeGameOver = 2;
        public GameObject GameOverScreenUi;
        public TryAgainOverlay TryAgainOverlay;
        private GameStateManager gameStateManager;

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

        private void OnDestroy()
        {
            if (gameStateManager != null)
            {
                gameStateManager.GameOver -= GameStateManager_GameOver;
                gameStateManager.GameReloaded -= GameStateManager_GameReloaded;
            }
        }

        private void GameStateManager_GameReloaded(object sender, EventArgs e)
        {
            GameOverScreenUi.SetActive(false);
        }

        private void GameStateManager_GameOver(object sender, EventArgs e)
        {
            StartCoroutine(StartGameOverAnimation());
        }

        private IEnumerator StartGameOverAnimation()
        {
            yield return new WaitForSecondsRealtime(TimeBeforeGameOver);
            GameOverScreenUi.SetActive(true);
            GameOverScreenUi.GetComponent<Animation>().Play();
        }

        public void OnTryAgain()
        {
            TryAgainOverlay.StartTryAgainAnimation();
        }

        public void OnGiveUp()
        {
            Application.Quit();
        }
    }
}
