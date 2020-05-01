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
    class TutorialStepStartFight : TutorialStepWithMessageBoxBase
    {
        private CombatantsManager combatantsManager;
        protected override void Start()
        {
            base.Start();
            combatantsManager = FindObjectOfType<CombatantsManager>();
            foreach (var interactableObject in FindObjectsOfType<InteractableObject>())
            {
                interactableObject.IsInteractionDisabledByTutorial = false;
            }

        }

        private void Update()
        {
            if (!completedTutorialAction && combatantsManager.IsCombatActive)
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
