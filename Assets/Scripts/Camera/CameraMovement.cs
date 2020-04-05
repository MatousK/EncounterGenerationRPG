﻿using Assets.Scripts.Combat;
using Assets.Scripts.Environment;
using UnityEngine;

namespace Assets.Scripts.Camera
{
    public class CameraMovement : MonoBehaviour
    {
        public Hero FollowingHero;
        public float MaxSquaredDistanceFromMap = 2;
        public float ScrollingEdge = 50;
        public float ScrollingSpeed = 0.1f;
        // Used when we are quickly zooming to some specified hero.
        public float QuickScrollingSpeed = 100f;
        bool isQuickFindHeroInProgress;
        FogOfWarController fogOfWarController;

        private void Awake()
        {
            fogOfWarController = FindObjectOfType<FogOfWarController>();
        }

        // Update is called once per frame
        void LateUpdate()
        {
            Vector3 oldPosition = transform.position;
            if (isQuickFindHeroInProgress && FollowingHero != null)
            {
                FollowHeroIfPossible();
                if (Vector2.Distance(transform.position, FollowingHero.transform.position) < 0.1)
                {
                    isQuickFindHeroInProgress = false;
                    FollowingHero = null;
                }
                return;
            }
            var mouseY = UnityEngine.Input.mousePosition.y;
            var mouseX = UnityEngine.Input.mousePosition.x;

            if (mouseX < 0 || mouseY < 0 || mouseX > Screen.width || mouseY > Screen.height)
            {
                // Mouse out of bounds;
                return;
            }
            if (mouseX < ScrollingEdge)
            {
                FollowingHero = null;
                transform.Translate(new Vector3(-ScrollingSpeed * Time.unscaledDeltaTime, 0, 0));
            }
            else if (mouseX > Screen.width - ScrollingEdge)
            {
                FollowingHero = null;
                transform.Translate(new Vector3(ScrollingSpeed * Time.unscaledDeltaTime, 0, 0));
            }
            if (mouseY < ScrollingEdge)
            {
                FollowingHero = null;
                transform.Translate(new Vector3(0, -ScrollingSpeed * Time.unscaledDeltaTime, 0));
            }
            else if (mouseY > Screen.height - ScrollingEdge)
            {
                FollowingHero = null;
                transform.Translate(new Vector3(0, ScrollingSpeed * Time.unscaledDeltaTime, 0));
            }
            FollowHeroIfPossible();
            UndoCameraMovementIfOutOfBounds(oldPosition);
        }

        public void QuickFindHero(Hero hero)
        {
            FollowingHero = hero;
            isQuickFindHeroInProgress = true;
        }

        private void UndoCameraMovementIfOutOfBounds(Vector3 oldPosition)
        {
            if (fogOfWarController.ExploredAreaBounds != null)
            {
                var exploredAreaBounds = fogOfWarController.ExploredAreaBounds.Value;
                // Make sure the Z coordinate is within bounds, we do not care about z coordinates.
                var cameraPosition = new Vector3(transform.position.x, transform.position.y, exploredAreaBounds.center.z);
                if (exploredAreaBounds.SqrDistance(cameraPosition) > MaxSquaredDistanceFromMap)
                {
                    transform.position = oldPosition;
                }
            }
        }

        private void FollowHeroIfPossible()
        {
            if (FollowingHero == null)
            {
                return;
            }
            var speed = isQuickFindHeroInProgress ? QuickScrollingSpeed : ScrollingSpeed;
            var newPosition = Vector2.MoveTowards(transform.position, FollowingHero.transform.position, speed * Time.unscaledDeltaTime);
            transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
        }
    }
}
