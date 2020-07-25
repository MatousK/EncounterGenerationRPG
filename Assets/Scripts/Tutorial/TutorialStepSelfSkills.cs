using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Combat;

namespace Assets.Scripts.Tutorial
{
    /// <summary>
    /// <inheritdoc/>
    /// This step teaches player about personal skill. Ends when the user activates the healing aura ability
    /// </summary>
    class TutorialStepSelfSkills : TutorialStepWithMessageBoxBase
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void Start()
        {
            base.Start();
        }
        /// <summary>
        /// Called by the skill when used. Ends this tutorial step.
        /// </summary>
        public void HealingAuraUsed()
        {
            if (!completedTutorialAction)
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
