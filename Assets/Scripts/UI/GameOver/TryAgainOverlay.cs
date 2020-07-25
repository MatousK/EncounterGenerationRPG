using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.GameFlow;
using UnityEngine;

namespace Assets.Scripts.UI.GameOver
{
    /// <summary>
    /// The overlay fades the screen to white hiding everything, restarts this dungeon floor and then fades back.
    /// </summary>
    public class TryAgainOverlay: MonoBehaviour
    {
        /// <summary>
        /// The component we use to trigger the reload.
        /// </summary>
        private GameStateManager gameStateManager;

        /// <summary>
        /// Called before the first Update, initializes references to dependencies.
        /// </summary>
        private void Start()
        {
            gameStateManager = FindObjectOfType<GameStateManager>();
        }
        /// <summary>
        /// Starts the animation, during which the level will be reloaded.
        /// </summary>
        public void StartTryAgainAnimation()
        {
            gameObject.SetActive(true);
            GetComponent<Animation>().Play();
        }
        /// <summary>
        /// Called when the animation is finished. Disables the object to which the animation is attached.
        /// </summary>
        public void TryAgainAnimationEnded()
        {
            gameObject.SetActive(false);
        }
        /// <summary>
        /// Called from the animation when it fates completely to white. Reloads the dungeon.
        /// </summary>
        public void OnFadeInComplete()
        {
            gameStateManager.OnGameReloaded();
        }
    }
}
