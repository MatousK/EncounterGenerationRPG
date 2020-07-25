using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Tutorial
{
    /// <summary>
    /// <inheritdoc/>
    /// This step teaches the player about camera movement. He must move the camera around a bit.
    /// </summary>
    public class TutorialStepCameraMovement: TutorialStepWithMessageBoxBase
    {
        /// <summary>
        /// How much must the player move the camera to finish the step.
        /// </summary>
        public float CameraDistanceToEndStep = 10f;
        /// <summary>
        /// How much has the player already moved the camera.
        /// </summary>
        private float currentTraveledCameraDistance = 0;
        /// <summary>
        /// Where was the camera last frame.
        /// </summary>
        private Vector3? lastCameraPosition;
        /// <summary>
        /// The camera being moved.
        /// </summary>
        private UnityEngine.Camera mainCamera;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void Start()
        {
            base.Start();
            mainCamera = FindObjectOfType<UnityEngine.Camera>();
        }
        /// <summary>
        /// Tracks how much has the player moved the camera. Once enough, end the step.
        /// </summary>
        private void Update()
        {
            if (didMessageBoxAppear)
            {
                if (lastCameraPosition != null)
                {
                    currentTraveledCameraDistance +=
                        Vector3.Distance(lastCameraPosition.Value, mainCamera.transform.position);
                }
                lastCameraPosition = mainCamera.transform.position;
                if (currentTraveledCameraDistance > CameraDistanceToEndStep && !completedTutorialAction)
                {
                    messageBox.Hide();
                    completedTutorialAction = true;
                }
            }
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}
