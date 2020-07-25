using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Combat;
using Assets.Scripts.Environment;

namespace Assets.Scripts.Tutorial
{
    /// <summary>
    /// <inheritdoc/>
    /// Meant to teach the player about right clicking. Ends when the user opens the doors out of the starting room.
    /// </summary>
    class TutorialStepRightClick : TutorialStepWithMessageBoxBase
    {
        private Doors[] allDoors;
        /// <summary>
        /// <inheritdoc/>
        /// Enables left and right clicking.
        /// </summary>
        protected override void Start()
        {
            base.Start();
            GetComponentInParent<TutorialController>().LeftClickController.enabled = true;
            GetComponentInParent<TutorialController>().RightClickController.enabled = true;
            allDoors = FindObjectsOfType<Doors>();

        }
        /// <summary>
        /// Executed every frame. If it finds some opened doors, ends the current step.
        /// </summary>
        private void Update()
        {
            if (!completedTutorialAction && allDoors.Any(door => door.IsOpened))
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
