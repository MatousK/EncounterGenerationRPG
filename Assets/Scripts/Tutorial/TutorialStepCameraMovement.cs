using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Tutorial
{
    public class TutorialStepCameraMovement: TutorialStepWithMessageBoxBase
    {
        public float CameraDistanceToEndStep = 10f;
        private float currentTraveledCameraDistance = 0;
        private Vector3? lastCameraPosition;
        private UnityEngine.Camera mainCamera;


        protected override void Start()
        {
            base.Start();
            mainCamera = FindObjectOfType<UnityEngine.Camera>();
        }

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

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}
