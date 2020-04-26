using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Input;
using UnityEngine;

namespace Assets.Scripts.Tutorial
{
    class TutorialController: MonoBehaviour
    {
        public List<TutorialStep> AllTutorialSteps;
        [HideInInspector]
        public LeftClickController LeftClickController;
        [HideInInspector]
        public RightClickController RightClickController;

        private int currentTutorialStep = 0;
        private bool isTutorialActive;

        public void Update()
        {
            if (isTutorialActive && AllTutorialSteps[currentTutorialStep].IsTutorialStepOver())
            {
                AllTutorialSteps[currentTutorialStep].gameObject.SetActive(false);
                ++currentTutorialStep;
                ExecuteCurrentTutorialStep();
            }
        }

        public void StartTutorial()
        {
            LeftClickController = FindObjectOfType<LeftClickController>();
            RightClickController = FindObjectOfType<RightClickController>();
            LeftClickController.enabled = false;
            RightClickController.enabled = false;
            isTutorialActive = true;
            currentTutorialStep = 0;
            ExecuteCurrentTutorialStep();
        }

        public void EndTutorial()
        {
            LeftClickController.enabled = true;
            RightClickController.enabled = true;
            isTutorialActive = false;
        }

        private void ExecuteCurrentTutorialStep()
        {
            if (AllTutorialSteps == null || currentTutorialStep >= AllTutorialSteps.Count)
            {
                EndTutorial();
                return;
            }
            AllTutorialSteps[currentTutorialStep].gameObject.SetActive(true);
        }
    }
}
