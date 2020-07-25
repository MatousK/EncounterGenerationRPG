using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Input;

namespace Assets.Scripts.Tutorial
{
    /// <summary>
    /// <inheritdoc/>
    /// This step teaches the player about pausing the game. It will end when the user pauses and unpauses the game.
    /// </summary>
    class TutorialStepPause : TutorialStepWithMessageBoxBase
    {
        private PauseManager pauseManager;
        private bool didPause;

        /// <summary>
        /// <inheritdoc/>
        /// Enables pause and blocks all doors to block the player from proceeding.
        /// </summary>
        protected override void Start()
        {
            base.Start();
            pauseManager = GetComponentInParent<TutorialController>().PauseManager;
            pauseManager.enabled = true;
            foreach (var interactableObject in FindObjectsOfType<InteractableObject>())
            {
                interactableObject.IsInteractionDisabledByTutorial = true;
            }

        }
        /// <summary>
        /// Executed every frame. If the game is unpaused and was paused, end the step.
        /// </summary>
        private void Update()
        {
            if (didPause && !pauseManager.IsPausedByPlayer && !completedTutorialAction)
            {
                completedTutorialAction = true;
                messageBox.Hide();
            }
            didPause = didPause || pauseManager.IsPausedByPlayer;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}
