﻿using Assets.Scripts.Combat;
using UnityEngine;

namespace Assets.Scripts.UI
{
    [ExecuteAfter(typeof(CommandConfirmationIndicator))]
    public class Circle : MonoBehaviour
    {
        public bool IsVisible = true;
        public Material LineMaterial;
        public int Segments = 360;
        public float Radius;
        public float LineWidth;
        LineRenderer line;
        float? lastFrameRadius;
        int? lastFrameSegments;

        // Start is called before the first frame update
        void Start()
        {
            DrawCircle(gameObject);
        }

        // Update is called once per frame
        void Update()
        {
            UpdateCircleProperties();
            line.enabled = IsVisible;
            lastFrameRadius = Radius;
            lastFrameSegments = Segments;
        }

        private void DrawCircle(GameObject container)
        {
            line = GetComponent<LineRenderer>();
            if (line == null)
            {
                line = container.AddComponent<LineRenderer>();
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

        private void UpdateCircleProperties()
        {
            line.startWidth = LineWidth;
            line.endWidth = LineWidth;
            if (!lastFrameRadius.HasValue || !lastFrameSegments.HasValue || lastFrameSegments.Value != Segments || lastFrameRadius.Value != Radius)
            {
                UpdateCirclePoints();
            }
        }

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
