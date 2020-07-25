using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.UI;
using GeneralAlgorithms.Algorithms.Common;
using UnityEngine;

namespace Assets.Scripts.Tutorial
{
    /// <summary>
    /// <inheritdoc/>
    /// This is a generic component for a step which only displays the message and can be dismissed by clicking anywhere.
    /// </summary>
    public class TutorialStepSimpleMessage: TutorialStepWithMessageBoxBase
    {

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void Start()
        {
            base.Start();
        }
        /// <summary>
        /// Executed every frame. Dismisses the message box if the player is presses anything.
        /// </summary>
        private void Update()
        {
            if (didMessageBoxAppear && UnityEngine.Input.anyKeyDown && !completedTutorialAction)
            {
                completedTutorialAction = true;
                messageBox.Hide();
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
