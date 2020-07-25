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

    /// <summary>
    /// This will check whether there is currently a character under the cursor and it will highlight it if possible.
    /// Why the execute after attribute - HighlightableObject's update will always clear the IsHighlighted flag, of that class LateUpdate will apply it.
    /// This way this controller can only highlight the things under the cursor, as everything is unhighlighted every frame.
    /// </summary>
    [ExecuteAfter(typeof(HighlightableObject))]
    class MouseOverHighlightController: MonoBehaviour
    {
        /// <summary>
        /// Called every frame. Checks if there is something highlightable under the cursor. If yes, highlight it.
        /// </summary>
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
