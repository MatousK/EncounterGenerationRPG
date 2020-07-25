using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Combat;
using Assets.Scripts.DungeonGenerator;
using Assets.Scripts.Environment;
using Assets.Scripts.Input;

namespace Assets.Scripts.Tutorial
{
    /// <summary>
    /// <inheritdoc/>
    /// This step gives the player info about enemy skills and then sends him to combat.
    /// This step will end when the player stats a combat.
    /// </summary>
    class TutorialStepStartFight : TutorialStepWithMessageBoxBase
    {
        /// <summary>
        /// The class which knows about all combatants in the game. Used to detect start of combat.
        /// </summary>
        private CombatantsManager combatantsManager;
        /// <summary>
        /// <inheritdoc/>
        /// Enables doors so the player can enter the room with combat.
        /// </summary>
        protected override void Start()
        {
            base.Start();
            combatantsManager = FindObjectOfType<CombatantsManager>();
            foreach (var interactableObject in FindObjectsOfType<InteractableObject>())
            {
                interactableObject.IsInteractionDisabledByTutorial = false;
            }

        }
        /// <summary>
        /// Executed every frame. Once a combat is over, ends this tutorial step.
        /// </summary>
        private void Update()
        {
            if (!completedTutorialAction && combatantsManager.IsCombatActive)
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
