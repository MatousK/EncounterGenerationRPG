using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Combat;

namespace Assets.Scripts.Tutorial
{
    class TutorialStepSelfSkills : TutorialStepWithMessageBoxBase
    {
        protected override void Start()
        {
            base.Start();

        }

        public void HealingAuraUsed()
        {
            if (!completedTutorialAction)
            {
                messageBox.Hide();
                completedTutorialAction = true;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}
