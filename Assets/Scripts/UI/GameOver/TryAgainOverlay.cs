using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.GameFlow;
using UnityEngine;

namespace Assets.Scripts.UI.GameOver
{
    public class TryAgainOverlay: MonoBehaviour
    {
        private GameStateManager gameStateManager;

        private void Awake()
        {
            gameStateManager = FindObjectOfType<GameStateManager>();
        }

        public void StartTryAgainAnimation()
        {
            gameObject.SetActive(true);
            GetComponent<Animation>().Play();
        }

        public void TryAgainAnimationEnded()
        {
            gameObject.SetActive(false);
        }

        public void OnFadeInComplete()
        {
            gameStateManager.OnGameReloaded();
        }
    }
}
