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
        [HideInInspector]
        public PauseManager PauseManager;

        /// <summary>
        /// If true, we should start the tutorial automatically on level load. Mainly used for testing.
        /// </summary>
        public bool AutoStart;

        private int currentTutorialStep = 0;
        private bool isTutorialActive;

        private void Start()
        {
            if (AutoStart)
            {
                StartTutorial();
            }
        }

        private void Update()
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
            PauseManager = FindObjectOfType<PauseManager>();
            LeftClickController.enabled = false;
            RightClickController.enabled = false;
            PauseManager.enabled = false;
            isTutorialActive = true;
            currentTutorialStep = 0;
            ExecuteCurrentTutorialStep();
        }

        private void EndTutorial()
        {
            LeftClickController.enabled = true;
            RightClickController.enabled = true;
            PauseManager.enabled = true;
            isTutorialActive = false;
            foreach (var interactableObject in FindObjectsOfType<InteractableObject>())
            {
                interactableObject.IsInteractionDisabledByTutorial = false;
            }
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
