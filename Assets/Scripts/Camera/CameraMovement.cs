using Assets.Scripts.Combat;
using Assets.Scripts.Cutscenes;
using Assets.Scripts.Environment;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Camera
{
    /// <summary>
    /// Class that can move the camera around slowly. It responds to the cursor being at the edges of the screen and to commands from the rest of the game.
    /// </summary>
    public class CameraMovement : MonoBehaviour
    {
        /// <summary>
        /// The target on which the camera should be centered.
        /// </summary>
        public Transform FollowingTransform;
        /// <summary>
        /// How far from the currently explored area can the camera move. Squared for easy comparison without calculating roots.
        /// </summary>
        public float MaxSquaredDistanceFromMap = 2;
        /// <summary>
        /// How many pixels near the edge of the screen must the cursor be for scrolling.
        /// </summary>
        public float ScrollingEdge = 50;
        /// <summary>
        /// How quickly can the camera scroll. 
        /// </summary>
        public float ScrollingSpeed = 0.1f;
        /// <summary>
        /// Faster scrolling speed, this is used when we need to quickly pan to the hero when the user double clicks on the portrait.
        /// </summary>
        public float QuickScrollingSpeed = 100f;
        /// <summary>
        /// If true, the camera is trying to quickly pan to the specified transform. <see cref="FollowingTransform"/>.
        /// </summary>
        bool isQuickFindHeroInProgress;
        /// <summary>
        /// Component which knows about the current state of fog of war. Necessary because the camera should stay in the explored regions.
        /// </summary>
        FogOfWarController fogOfWarController;
        /// <summary>
        /// Component that knows whether a cutscene is playing. Needed because we need to disable scrolling during cutscenes.
        /// </summary>
        private CutsceneManager cutsceneManager;

        private void Update()
        {
            // As camera works by scrolling to the edge, we need to lock the cursor in the window. Without this the cursor could leave the game window.
            Cursor.lockState = CursorLockMode.Confined;
        }

        // LateUpdate is used so the camera more swiftly responds to e.g. double clicking on a hero portrait (so it is executed after the double click is detected).
        void LateUpdate()
        {
            // Bit hacky, basically camera exists even before these managers. So it must keep trying to get a reference until it succeeds.
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
            // Detect whether the user is scrolling the camera. Manual movement interrupts following of something else.
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
        /// <summary>
        /// Quickly pans to the specified hero.
        /// </summary>
        /// <param name="hero">The hero we should pan to.</param>
        public void QuickFindHero(Hero hero)
        {
            FollowingTransform = hero.transform;
            isQuickFindHeroInProgress = true;
        }
        /// <summary>
        /// If some movement method move the camera outside of the explored bounds, put it back where it started.
        /// </summary>
        /// <param name="oldPosition">Position where the camera started.</param>
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
        /// <summary>
        /// If there is some hero set we should follow, follow him. Quickly or slowly based on class state, see <see cref="isQuickFindHeroInProgress"/>
        /// </summary>
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
