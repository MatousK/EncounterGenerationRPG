using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftClickController : MonoBehaviour
{
    private SelectionController selectionController;
    Texture2D whiteTexture;
    Rect currentSelectionRectangle = Rect.zero;
    Vector2 selectionStart = Vector2.positiveInfinity;
    // Start is called before the first frame update
    void Start()
    {
        whiteTexture = new Texture2D(1, 1);
        whiteTexture.SetPixel(0, 0, Color.white);
        whiteTexture.Apply();
        selectionController = GetComponent<SelectionController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Start dragging selection box;
            selectionStart = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            SelectPlayerCharacters(selectionStart, Input.mousePosition);
            // End dragging selection box
            selectionStart = Vector2.positiveInfinity;
            currentSelectionRectangle = Rect.zero;
        }
        if (!float.IsInfinity(selectionStart.x))
        {
            currentSelectionRectangle = GetScreenRect(selectionStart, Input.mousePosition);
        }
    }

    private void SelectPlayerCharacters(Vector2 selectionStart, Vector2 selectionEnd)
    {
        var selectionBoxBounds = GetWorldBounds(selectionStart, selectionEnd);
        foreach (var character in selectionController.PlayableCharacters)
        {
            var characterBounds = character.GetComponent<SpriteRenderer>().bounds;
            character.GetComponent<SelectableObject>().IsSelected = characterBounds.Intersects(selectionBoxBounds);
        }
    }
    // MARK: Screen space transformations.
    Bounds GetWorldBounds(Vector2 screenPosition1, Vector2 screenPosition2)
    {
        var worldPosition1 = Camera.main.ScreenToWorldPoint(screenPosition1);
        var worldPosition2 = Camera.main.ScreenToWorldPoint(screenPosition2);
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
