using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Input;
using UnityEngine;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// Indicator which shows that the game is paused right now.
    /// </summary>
    public class PauseIndicator: MonoBehaviour
    {
        /// <summary>
        /// Object which should be enabled while the game is paused.
        /// </summary>
        public GameObject TextGameObject;
        /// <summary>
        /// Component which knows whether the game is paused.
        /// </summary>
        private PauseManager pauseManager;

        /// <summary>
        /// Called before first Update. Finds a reference to the <see cref="PauseManager"/>.
        /// </summary>
        private void Start()
        {
            pauseManager = FindObjectOfType<PauseManager>();
        }
        /// <summary>
        /// Called every frame. Updates the visibility of the pause indicator.
        /// </summary>
        private void Update()
        {
            if (pauseManager.IsPaused != TextGameObject.activeSelf)
            {
                TextGameObject.SetActive(pauseManager.IsPaused);
            }
        }
    }
}
