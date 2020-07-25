using System.Linq;
using Assets.Scripts.Combat;
using Assets.Scripts.Cutscenes;
using Assets.Scripts.Sound.CharacterSounds;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Input
{
    /// <summary>
    /// The component which handles all left clicks of the player.
    /// Does not handle UI.
    /// </summary>
    public class LeftClickController : MonoBehaviour
    {
        /// <summary>
        /// The class which knows about all combatants in the game.
        /// </summary>
        private CombatantsManager combatantsManager;
        /// <summary>
        /// This component handles the user clicking on the UI icon representing a skill.
        /// We need a reference to it because if the user left clicks somewhere, the skill is not being used anymore.
        /// </summary>
        private SkillFromUiIconClickController skillFromUiIconClickController;
        /// <summary>
        /// A blank texture which can then be colored to draw the rectangle over the player characters.
        /// </summary>
        Texture2D whiteTexture;
        /// <summary>
        /// The rectangle the player is currently drawing by dragging while left clicking.
        /// </summary>
        Rect currentSelectionRectangle = Rect.zero;
        /// <summary>
        /// The point at which the user started holding the left mouse button.
        /// </summary>
        Vector2? selectionStart;
        /// <summary>
        /// The component responsible for cutscenes. It can block UI.
        /// </summary>
        CutsceneManager cutsceneManager;
        private void Awake()
        {
            whiteTexture = new Texture2D(1, 1);
            whiteTexture.SetPixel(0, 0, Color.white);
            whiteTexture.Apply();
            skillFromUiIconClickController = GetComponent<SkillFromUiIconClickController>();
        }
        /// <summary>
        /// Start is called before the first frame update. Gets references to dependencies.
        /// </summary>
        private void Start()
        {
            combatantsManager = FindObjectOfType<CombatantsManager>();
            cutsceneManager = FindObjectOfType<CutsceneManager>();
        }
        /// <summary>
        /// If something, e.g. the tutorial, disabled left clicking while we were doing some selection, this object would not clear selectionStart on release of mouse button.
        /// Because it was disabled.
        /// So on enable we clear the selection start to indicate that no, the player is not dragging.
        /// </summary>
        private void OnEnable()
        {
            selectionStart = null;
        }

        /// <summary>
        /// Update is called once per frame. Checks the current state of the left mouse button and appropriately selects characters and draws selection rectangles.
        /// </summary>
        private void Update()
        {
            if (cutsceneManager.IsCutsceneActive)
            {
                return;
            }

            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                if (skillFromUiIconClickController.IsUsingSkill)
                {
                    // Left click somewhere invalid, stop skill usage and do nothing else.
                    skillFromUiIconClickController.ClearUsedSkill();
                    return;
                }

                if (EventSystem.current.IsPointerOverGameObject())
                {
                    // Pointer is over UI, let UI handle this.
                    return;
                }
                // Start drawing selection box;
                selectionStart = UnityEngine.Input.mousePosition;
            }
            else if (UnityEngine.Input.GetMouseButtonUp(0))
            {
                if (!selectionStart.HasValue)
                {
                    // Drag ended without starting. So do nothing to be sure.
                    return;
                }
                // Ok, so this might be a bit hacky.
                // Basically, on one hand we do not want clicks over UI to deselect or select characters.
                // On the other hand, if the we are dragging a selection rectangle, we want to select, as the click might
                // have originated outside of the UI. So we allow selection to continue if we have some rectangle worth mentioning.
                if (!EventSystem.current.IsPointerOverGameObject() || currentSelectionRectangle.size.magnitude > 0.5)
                {
                    // Selects the characters in the selection rectangle.
                    SelectPlayerCharacters(selectionStart.Value, UnityEngine.Input.mousePosition);
                }
                // End dragging selection box
                selectionStart = null;
                currentSelectionRectangle = Rect.zero;
                // You might have noticed that there is no special handling for only clicking without dragging.
                // But that does not matter, for us a click is also a drag. The dragging rectangle does not have to completely
                // include the selected game characters, so a tiny rectangle inside the character, even with zero size, will still select him.
            }
            if (selectionStart != null)
            {
                // Still dragging, update the green selection rectangle to draw.
                currentSelectionRectangle = GetScreenRect(selectionStart.Value, UnityEngine.Input.mousePosition);
            }
        }
        /// <summary>
        /// Select all characters that are in the given area.
        /// </summary>
        /// <param name="selectionStart">The mouse position where dragging started.</param>
        /// <param name="selectionEnd">The mouse position where dragging ended.</param>
        private void SelectPlayerCharacters(Vector2 selectionStart, Vector2 selectionEnd)
        {
            // First we must translate the rectangle from mouse positions to world space.
            var selectionBoxBounds = GetWorldBounds(selectionStart, selectionEnd);
            
            // Try to select all characters in the area who are alive and selectable. Also deselect characters outside of this area if not holding shift.
            foreach (var character in combatantsManager.GetPlayerCharacters(onlyAlive: true))
            {
                var selectableComponent = character.GetComponent<SelectableObject>();
                if (selectableComponent.IsSelected && (UnityEngine.Input.GetKey(KeyCode.LeftShift) || UnityEngine.Input.GetKey(KeyCode.RightShift)))
                {
                    // If selecting while holding shift, we do not want to deselect other characters. And this one is already selected.
                    continue;
                }
                if (!selectableComponent.IsSelectionEnabled)
                {
                    continue;
                }
                var characterBounds = character.GetComponent<SpriteRenderer>().bounds;
                selectableComponent.IsSelected = characterBounds.Intersects(selectionBoxBounds);
            }
            // If some character is selected, play the greetings sound effect.
            var selectedCharacter = combatantsManager.GetPlayerCharacters(onlySelected: true).FirstOrDefault();
            if (selectedCharacter != null)
            {
                selectedCharacter.GetComponentInChildren<CharacterVoiceController>().PlayOnSelectedSound();
            }
        }
        #region Screen space transformations.
        /// <summary>
        /// Calculates the world space bounds of the screen space rectangle.
        /// </summary>
        /// <param name="screenPosition1">One corner of the rectangle drawn by the mouse.</param>
        /// <param name="screenPosition2">The opposite corner of the rectangle drawn by the mouse.</param>
        /// <returns></returns>
        Bounds GetWorldBounds(Vector2 screenPosition1, Vector2 screenPosition2)
        {
            var worldPosition1 = UnityEngine.Camera.main.ScreenToWorldPoint(screenPosition1);
            var worldPosition2 = UnityEngine.Camera.main.ScreenToWorldPoint(screenPosition2);
            var min = Vector3.Min(worldPosition1, worldPosition2);
            var max = Vector3.Max(worldPosition1, worldPosition2);
            // We are interested only in 2D intersections, so we give these huge Z values range to cover the entire scene.
            min.z = -1000;
            max.z = 1000;
            var bounds = new Bounds();
            bounds.SetMinMax(min, max);
            return bounds;
        }
        /// <summary>
        /// Get the rectangle between the two mouse positions.
        /// </summary>
        /// <param name="screenPosition1">One corner of the rectangle.</param>
        /// <param name="screenPosition2">Second corner of the rectangle.</param>
        /// <returns></returns>
        private Rect GetScreenRect(Vector2 screenPosition1, Vector2 screenPosition2)
        {
            // Move origin from bottom left to top left
            screenPosition1.y = Screen.height - screenPosition1.y;
            screenPosition2.y = Screen.height - screenPosition2.y;
            // Calculate corners
            var topLeft = Vector3.Min(screenPosition1, screenPosition2);
            var bottomRight = Vector3.Max(screenPosition1, screenPosition2);
            // Create Rect
            return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
        }
        #endregion
        #region Selection box Drawing
        /// <summary>
        /// Called when we should draw GUI stuff.
        /// </summary>
        private void OnGUI()
        {
            if (currentSelectionRectangle != Rect.zero)
            {
                DrawBorder(currentSelectionRectangle, 2, Color.green);
            }
        }
        /// <summary>
        /// Draw the border of the specified rectangle.
        /// </summary>
        /// <param name="rectangle">Rectangle to draw.</param>
        /// <param name="thickness">Thickness of the border to draw.</param>
        /// <param name="color">Color of the border.</param>
        private void DrawBorder(Rect rectangle, float thickness, Color color)
        {
            // Top border
            DrawRectangle(new Rect(rectangle.xMin, rectangle.yMin, rectangle.width, thickness), color);
            // Left border
            DrawRectangle(new Rect(rectangle.xMin, rectangle.yMin, thickness, rectangle.height), color);
            // Right border
            DrawRectangle(new Rect(rectangle.xMax - thickness, rectangle.yMin, thickness, rectangle.height), color);
            // Bottom border
            DrawRectangle(new Rect(rectangle.xMin, rectangle.yMax - thickness, rectangle.width, thickness), color);
        }
        /// <summary>
        /// Draw a rectangle on the screen.
        /// </summary>
        /// <param name="rectangle">Rectangle to draw.</param>
        /// <param name="color">Color of the rectangle.</param>
        private void DrawRectangle(Rect rectangle, Color color)
        {
            GUI.color = color;
            GUI.DrawTexture(rectangle, whiteTexture);
            GUI.color = Color.white;
        }
        #endregion
    }
}
