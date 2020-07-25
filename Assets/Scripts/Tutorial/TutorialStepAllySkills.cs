using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Tutorial
{
    /// <summary>
    /// <inheritdoc/>
    /// This step teaches player about using ally skills. He must use Heal Other ability of the cleric.
    /// </summary>
    class TutorialStepAllySkills : TutorialStepWithMessageBoxBase
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void Start()
        {
            base.Start();

        }
        /// <summary>
        /// Call this method when the heal other ability is used.
        /// </summary>
        public void HealOtherUsed()
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
