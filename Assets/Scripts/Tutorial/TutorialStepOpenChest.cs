using Assets.Scripts.Environment;
using Assets.Scripts.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Tutorial
{
    /// <summary>
    /// <inheritdoc/>
    /// This tutorial step will end when the player opens a chest.
    /// Meant to teach to player to open chests.
    /// </summary>
    class TutorialStepOpenChest : TutorialStepWithMessageBoxBase
    {
        /// <summary>
        /// All chests on the map.
        /// </summary>
        private TreasureChest[] allChests;
        /// <summary>
        /// <inheritdoc/>
        /// Also blocks usage of doors to force the player to open a chest before proceeding.
        /// </summary>
        protected override void Start()
        {
            base.Start();
            allChests = FindObjectsOfType<TreasureChest>();
            // Disable all doors so the player does not leave;
            var doors = FindObjectsOfType<Doors>();
            foreach (var door in doors)
            {
                door.GetComponent<InteractableObject>().IsInteractionDisabledByTutorial = true;
            }
        }
        /// <summary>
        /// Executed every frame.Checks if there is some opened chest. If true, this tutorial step is over.
        /// </summary>
        private void Update()
        {
            if (!completedTutorialAction && allChests.Any(chest => chest.IsOpened))
            {
                messageBox.Hide();
                completedTutorialAction = true;
            }
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
