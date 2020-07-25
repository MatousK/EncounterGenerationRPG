using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Input;
using UnityEngine;

namespace Assets.Scripts.Tutorial
{
    /// <summary>
    /// Class that can play the tutorial for this game.
    /// </summary>
    class TutorialController: MonoBehaviour
    {
        /// <summary>
        /// The sequence of tutorial steps that should be executed.
        /// </summary>
        public List<TutorialStep> AllTutorialSteps;
        /// <summary>
        /// Left click controller, we keep a reference to it as we will disable it during the tutorial and we will need to find it again to reenable it.
        /// </summary>
        [HideInInspector]
        public LeftClickController LeftClickController;
        /// <summary>
        /// Right click controller, we keep a reference to it as we will disable it during the tutorial and we will need to find it again to reenable it.
        /// </summary>
        [HideInInspector]
        public RightClickController RightClickController;
        /// <summary>
        /// Pause click controller, we keep a reference to it as we will disable it during the tutorial and we will need to find it again to reenable it.
        /// </summary>
        [HideInInspector]
        public PauseManager PauseManager;

        /// <summary>
        /// If true, we should start the tutorial automatically on level load. Mainly used for testing.
        /// </summary>
        public bool AutoStart;
        /// <summary>
        /// The index of the currently executed tutorial step.
        /// </summary>
        private int currentTutorialStep = 0;
        /// <summary>
        /// If true, the tutorial is currently playing.
        /// </summary>
        private bool isTutorialActive;

        /// <summary>
        /// Called before the first Update, starts the tutorial if <see cref="AutoStart"/> is true.
        /// </summary>
        private void Start()
        {
            if (AutoStart)
            {
                StartTutorial();
            }
        }
        /// <summary>
        /// Called every frame. Checks whether the tutorial steps is over.
        /// If it is, move to the next step, ending the tutorial if we reached the end.
        /// </summary>
        private void Update()
        {
            if (isTutorialActive && AllTutorialSteps[currentTutorialStep].IsTutorialStepOver())
            {
                AllTutorialSteps[currentTutorialStep].gameObject.SetActive(false);
                ++currentTutorialStep;
                ExecuteCurrentTutorialStep();
            }
        }
        /// <summary>
        /// Plays the tutorial.
        /// </summary>
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
        /// <summary>
        /// Call when the tutorial ends to reenable everything that was disabled.
        /// </summary>
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
        /// <summary>
        /// Plays the tutorial step we are executing right now.
        /// </summary>
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
