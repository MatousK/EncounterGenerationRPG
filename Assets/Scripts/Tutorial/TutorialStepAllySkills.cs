using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Tutorial
{
    class TutorialStepAllySkills : TutorialStepWithMessageBoxBase
    {
        protected override void Start()
        {
            base.Start();

        }

        public void HealOtherUsed()
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
