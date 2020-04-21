using System.Linq;
using Assets.Scripts.Combat;
using Assets.Scripts.Cutscenes;
using Assets.Scripts.Sound.CharacterSounds;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Input
{
    public class LeftClickController : MonoBehaviour
    {
        private CombatantsManager combatantsManager;
        Texture2D whiteTexture;
        Rect currentSelectionRectangle = Rect.zero;
        Vector2? selectionStart;
        CutsceneManager cutsceneManager;
        // Start is called before the first frame update
        private void Awake()
        {
            whiteTexture = new Texture2D(1, 1);
            whiteTexture.SetPixel(0, 0, Color.white);
            whiteTexture.Apply();
        }

        private void Start()
        {
            combatantsManager = FindObjectOfType<CombatantsManager>();
            cutsceneManager = FindObjectOfType<CutsceneManager>();
        }

        // Update is called once per frame
        private void Update()
        {
            if (cutsceneManager.IsCutsceneActive)
            {
                return;
            }

            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                // Start dragging selection box;
                selectionStart = UnityEngine.Input.mousePosition;
            }
            else if (UnityEngine.Input.GetMouseButtonUp(0))
            {
                if (!selectionStart.HasValue)
                {
                    UnityEngine.Debug.Assert(false, "We should always first start dragging before ending dragging.");
                    return;
                }
                // Ok, so this might be a bit hacky.
                // Basically, on one hand we do not want clicks over UI to deselect or select characters.
                // On the other hand, if the we are dragging a selection rectangle, we want to select, as the click might
                // have originated outside of the UI. So we allow selection to continue if we have some rectangle worth mentioning.
                if (!EventSystem.current.IsPointerOverGameObject() || currentSelectionRectangle.size.magnitude > 0.5)
                {
                    SelectPlayerCharacters(selectionStart.Value, UnityEngine.Input.mousePosition);
                }
                // End dragging selection box
                selectionStart = null;
                currentSelectionRectangle = Rect.zero;
            }
            if (selectionStart != null)
            {
                currentSelectionRectangle = GetScreenRect(selectionStart.Value, UnityEngine.Input.mousePosition);
            }
        }

        private void SelectPlayerCharacters(Vector2 selectionStart, Vector2 selectionEnd)
        {
            var selectionBoxBounds = GetWorldBounds(selectionStart, selectionEnd);
            
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

            var selectedCharacter = combatantsManager.GetPlayerCharacters(onlySelected: true).FirstOrDefault();
            if (selectedCharacter != null)
            {
                selectedCharacter.GetComponentInChildren<CharacterVoiceController>().PlayOnSelectedSound();
            }
        }
        // MARK: Screen space transformations.
        Bounds GetWorldBounds(Vector2 screenPosition1, Vector2 screenPosition2)
        {
            var worldPosition1 = UnityEngine.Camera.main.ScreenToWorldPoint(screenPosition1);
            var worldPosition2 = UnityEngine.Camera.main.ScreenToWorldPoint(screenPosition2);
            var min = Vector3.Min(worldPosition1, worldPosition2);
            var max = Vector3.Max(worldPosition1, worldPosition2);
            min.z = -1000;
            max.z = 1000;
            var bounds = new Bounds();
            bounds.SetMinMax(min, max);
            return bounds;
        }
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
        // MARK: Selection box Drawing
        // Rectangle uses a different coordinate system than the screen space.
        private void OnGUI()
        {
            if (currentSelectionRectangle != Rect.zero)
            {
                DrawBorder(currentSelectionRectangle, 2, Color.green);
            }
        }

        private void DrawBorder(Rect rectangle, float thickness, Color color)
        {// Top
            DrawRectangle(new Rect(rectangle.xMin, rectangle.yMin, rectangle.width, thickness), color);
            // Left
            DrawRectangle(new Rect(rectangle.xMin, rectangle.yMin, thickness, rectangle.height), color);
            // Right
            DrawRectangle(new Rect(rectangle.xMax - thickness, rectangle.yMin, thickness, rectangle.height), color);
            // Bottom
            DrawRectangle(new Rect(rectangle.xMin, rectangle.yMax - thickness, rectangle.width, thickness), color);
        }

        private void DrawRectangle(Rect rectangle, Color color)
        {
            GUI.color = color;
            GUI.DrawTexture(rectangle, whiteTexture);
            GUI.color = Color.white;
        }
    }
}
