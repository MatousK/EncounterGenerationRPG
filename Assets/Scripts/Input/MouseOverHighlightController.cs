using Assets.Scripts.Combat;
using Assets.Scripts.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Input
{
    // HighlightableObject's update will clear the IsHighlighted flag, LateUpdate will apply it.
    [ExecuteAfter(typeof(HighlightableObject))]
    class MouseOverHighlightController: MonoBehaviour
    {

        private void Update()
        {
            // Raycast to see what's under cursor
            Vector3 mousePos = UnityEngine.Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

            var hitHighlightableObject = hit.collider != null ? hit.collider.GetComponent<HighlightableObject>() : null;

            if (hitHighlightableObject != null)
            {
                hitHighlightableObject.IsHighlighted = true;
            }
        }
    }
}
