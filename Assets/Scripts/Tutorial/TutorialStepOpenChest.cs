using Assets.Scripts.Environment;
using Assets.Scripts.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Tutorial
{
    class TutorialStepOpenChest : TutorialStepWithMessageBoxBase
    {
        private TreasureChest[] allChests;
        protected override void Start()
        {
            base.Start();
            allChests = FindObjectsOfType<TreasureChest>();
            // Disable all doors so the player does not leave;
            var doors = FindObjectsOfType<Doors>();
            foreach (var door in doors)
            {
                door.GetComponent<InteractableObject>().IsInteractionDisabledByTutorial = true;
            }
        }

        private void Update()
        {
            if (!completedTutorialAction && allChests.Any(chest => chest.IsOpened))
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
