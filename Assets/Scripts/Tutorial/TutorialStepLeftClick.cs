using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Combat;
using UnityEngine;

namespace Assets.Scripts.Tutorial
{
    class TutorialStepLeftClick : TutorialStepWithMessageBoxBase
    {
        private CombatantsManager combatantsManager;
        protected override void Start()
        {
            base.Start();
            combatantsManager = FindObjectOfType<CombatantsManager>();
            GetComponentInParent<TutorialController>().LeftClickController.enabled = true;
            GetComponentInParent<TutorialController>().RightClickController.enabled = false;

        }

        private void Update()
        {
            if (!completedTutorialAction && combatantsManager.GetPlayerCharacters(onlySelected: true).Count() ==
                combatantsManager.PlayerCharacters.Count)
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
