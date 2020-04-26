using Assets.Scripts.Combat;
using Assets.Scripts.Cutscenes;
using Assets.Scripts.Environment;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Camera
{
    public class CameraMovement : MonoBehaviour
    {
        public Transform FollowingTransform;
        public float MaxSquaredDistanceFromMap = 2;
        public float ScrollingEdge = 50;
        public float ScrollingSpeed = 0.1f;
        // Used when we are quickly zooming to some specified hero.
        public float QuickScrollingSpeed = 100f;
        bool isQuickFindHeroInProgress;
        FogOfWarController fogOfWarController;
        private CutsceneManager cutsceneManager;

        // Update is called once per frame
        void LateUpdate()
        {
            if (cutsceneManager == null || cutsceneManager == null)
            {
                cutsceneManager = FindObjectOfType<CutsceneManager>();
                fogOfWarController = FindObjectOfType<FogOfWarController>();
                if (cutsceneManager == null || cutsceneManager == null)
                {
                    // Map probably still loading, camera should not move at all.
                    return;
                }
            }
            Vector3 oldPosition = transform.position;
            var isCutsceneActive = cutsceneManager.IsCutsceneActive;
            if ((isQuickFindHeroInProgress || isCutsceneActive) && FollowingTransform != null)
            {
                FollowHeroIfPossible();
                if (Vector2.Distance(transform.position, FollowingTransform.position) < 0.1)
                {
                    isQuickFindHeroInProgress = false;
                    FollowingTransform = null;
                }
                return;
            }

            if (isCutsceneActive)
            {
                // Do not allow manual camera movement during a cutscene.
                return;
            }

            var mouseY = UnityEngine.Input.mousePosition.y;
            var mouseX = UnityEngine.Input.mousePosition.x;

            var mouseOverUi = EventSystem.current.IsPointerOverGameObject();

            if (mouseX < 0 || mouseY < 0 || mouseX > Screen.width || mouseY > Screen.height)
            {
                // Mouse out of bounds;
                return;
            }
            if (!mouseOverUi && mouseX < ScrollingEdge)
            {
                FollowingTransform = null;
                transform.Translate(new Vector3(-ScrollingSpeed * Time.unscaledDeltaTime, 0, 0));
            }
            else if (!mouseOverUi && mouseX > Screen.width - ScrollingEdge)
            {
                FollowingTransform = null;
                transform.Translate(new Vector3(ScrollingSpeed * Time.unscaledDeltaTime, 0, 0));
            }
            if (!mouseOverUi && mouseY < ScrollingEdge)
            {
                FollowingTransform = null;
                transform.Translate(new Vector3(0, -ScrollingSpeed * Time.unscaledDeltaTime, 0));
            }
            else if (!mouseOverUi && mouseY > Screen.height - ScrollingEdge)
            {
                FollowingTransform = null;
                transform.Translate(new Vector3(0, ScrollingSpeed * Time.unscaledDeltaTime, 0));
            }
            FollowHeroIfPossible();
            UndoCameraMovementIfOutOfBounds(oldPosition);
        }

        public void QuickFindHero(Hero hero)
        {
            FollowingTransform = hero.transform;
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
            if (FollowingTransform == null)
            {
                return;
            }
            var speed = isQuickFindHeroInProgress ? QuickScrollingSpeed : ScrollingSpeed;
            var newPosition = Vector2.MoveTowards(transform.position, FollowingTransform.position, speed * Time.unscaledDeltaTime);
            transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
        }
    }
}
