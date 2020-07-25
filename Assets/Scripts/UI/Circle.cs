using Assets.Scripts.Combat;
using UnityEngine;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// Draws a circle under a character.
    /// If there is some sprite renderer on this game object or parent, goes uses its sorting layer attributes. 
    /// This makes the circle appear under the character so he blocks it.
    /// </summary>
    [ExecuteAfter(typeof(CommandConfirmationIndicator))]
    public class Circle : MonoBehaviour
    {
        /// <summary>
        /// If true, this circle is visible right now.
        /// </summary>
        public bool IsVisible = true;
        /// <summary>
        /// The material used to draw the circle.
        /// </summary>
        public Material LineMaterial;
        /// <summary>
        /// Number of lines that should be used to draw the circle.
        /// </summary>
        public int Segments = 360;
        /// <summary>
        /// Radius of the circle.
        /// </summary>
        public float Radius;
        /// <summary>
        /// Width of the circle.
        /// </summary>
        public float LineWidth;
        /// <summary>
        /// The renderer used to drawn this circle.
        /// </summary>
        LineRenderer line;
        /// <summary>
        /// What was the radius in the last frame. If it changed, update the line representing the circle.
        /// </summary>
        float? lastFrameRadius;
        /// <summary>
        /// What was the number of segments which was representing this circle in the last frame.
        /// If it changed, update the line representing the circle.
        /// </summary>
        int? lastFrameSegments;

        /// <summary>
        /// Start is called before the first frame update.
        /// Draws the circle based on the class configuration.
        /// </summary>
        void Start()
        {
            DrawCircle();
        }

        /// <summary>
        /// Update is called once per frame. 
        /// Updates the drawn circle to fit the class configuration.
        /// </summary>
        void Update()
        {
            UpdateCircleProperties();
            line.enabled = IsVisible;
            lastFrameRadius = Radius;
            lastFrameSegments = Segments;
        }
        /// <summary>
        /// Draws the initial circle around the character.
        /// Adds the line renderer and initializes the correct sorting order and material.
        /// </summary>
        private void DrawCircle()
        {
            line = GetComponent<LineRenderer>();
            if (line == null)
            {
                line = gameObject.AddComponent<LineRenderer>();
            }
            var spriteRenderer = GetComponentInParent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                line.sortingLayerID = spriteRenderer.sortingLayerID;
                line.sortingOrder = spriteRenderer.sortingOrder - 1;
            }
            line.material = LineMaterial;
            line.useWorldSpace = false;
            UpdateCircleProperties();
        }
        /// <summary>
        /// Update the circle to fit the class settings.
        /// </summary>
        private void UpdateCircleProperties()
        {
            line.startWidth = LineWidth;
            line.endWidth = LineWidth;
            if (!lastFrameRadius.HasValue || !lastFrameSegments.HasValue || lastFrameSegments.Value != Segments || lastFrameRadius.Value != Radius)
            {
                UpdateCirclePoints();
            }
        }
        /// <summary>
        /// Updates the path that makes up the circle in the line renderer.
        /// </summary>
        void UpdateCirclePoints()
        {
            line.positionCount = Segments + 1;

            var pointCount = Segments + 1; // add extra point to make startpoint and endpoint the same to close the circle
            var points = new Vector3[pointCount];

            for (int i = 0; i < pointCount; i++)
            {
                var rad = Mathf.Deg2Rad * (i * 360f / Segments);
                points[i] = new Vector3(Mathf.Sin(rad) * Radius, Mathf.Cos(rad) * Radius, 0);
            }

            line.SetPositions(points);
        }
    }
}
