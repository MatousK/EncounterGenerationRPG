using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Input;

namespace Assets.Scripts.Tutorial
{
    class TutorialStepPause : TutorialStepWithMessageBoxBase
    {
        private PauseManager pauseManager;
        private bool didPause;

        protected override void Start()
        {
            base.Start();
            pauseManager = GetComponentInParent<TutorialController>().PauseManager;
            pauseManager.enabled = true;

        }

        private void Update()
        {
            if (didPause && !pauseManager.IsPausedByPlayer && !completedTutorialAction)
            {
                completedTutorialAction = true;
                messageBox.Hide();
            }
            didPause = didPause || pauseManager.IsPausedByPlayer;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}
