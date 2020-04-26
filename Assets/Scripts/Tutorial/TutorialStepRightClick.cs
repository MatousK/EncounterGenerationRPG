using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Combat;
using Assets.Scripts.Environment;

namespace Assets.Scripts.Tutorial
{
    class TutorialStepRightClick : TutorialStepWithMessageBoxBase
    {
        private CombatantsManager combatantsManager;
        private Doors[] allDoors;
        protected override void Start()
        {
            base.Start();
            combatantsManager = FindObjectOfType<CombatantsManager>();
            GetComponentInParent<TutorialController>().LeftClickController.enabled = true;
            GetComponentInParent<TutorialController>().RightClickController.enabled = true;
            allDoors = FindObjectsOfType<Doors>();

        }

        private void Update()
        {
            if (!completedTutorialAction && allDoors.Any(door => door.IsOpened))
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
