using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Combat;
using UnityEngine;

namespace Assets.Scripts.Tutorial
{
    /// <summary>
    /// <inheritdoc/>
    /// This step is meant to teach the player about selecting heroes and will end once the hero selects all his heroes.
    /// </summary>
    class TutorialStepLeftClick : TutorialStepWithMessageBoxBase
    {
        /// <summary>
        /// The component which knows about all heroes.
        /// </summary>
        private CombatantsManager combatantsManager;
        /// <summary>
        /// <inheritdoc/>
        /// Enables left clicking.
        /// </summary>
        protected override void Start()
        {
            base.Start();
            combatantsManager = FindObjectOfType<CombatantsManager>();
            GetComponentInParent<TutorialController>().LeftClickController.enabled = true;
            GetComponentInParent<TutorialController>().RightClickController.enabled = false;

        }
        /// <summary>
        /// Executed every frame, if all heroes are selected, end the tutorial step.
        /// </summary>
        private void Update()
        {
            if (!completedTutorialAction && combatantsManager.GetPlayerCharacters(onlySelected: true).Count() ==
                combatantsManager.PlayerCharacters.Count)
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
