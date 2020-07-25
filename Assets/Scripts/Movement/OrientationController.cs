using UnityEngine;

namespace Assets.Scripts.Movement
{
    /// <summary>
    /// This component can orient the character some way if he is fighting or moving.
    /// </summary>
    public class OrientationController : MonoBehaviour
    {
        /// <summary>
        /// If true, the sprite is mirrored, so our orientations should also be mirrored
        /// </summary>
        public bool IsSpriteMirrored = false;
        /// <summary>
        /// If set, velocity of this combatant will be ignored and the combatant will orient itself towards the target.
        /// </summary>
        public Vector2? LookAtTarget = null;
        /// <summary>
        /// If true, orienation should not change;
        /// </summary>
        public bool LockOrientation;
        /// <summary>
        /// Update is called once per frame. Calculates the orientation we should have and sets the x scale appropriately.
        /// </summary>
        void Update()
        {
            if (LockOrientation)
            {
                return;
            }
            var transformMultiplier = IsSpriteMirrored ? -1 : 1;
            // Calculate where should we look.
            Vector2 orientationVector;
            if (LookAtTarget == null)
            {
                orientationVector = GetComponent<VelocityManager>().GetVelocity();
            } else
            {
                Vector2 currentPosition = transform.position;
                orientationVector = LookAtTarget.Value - currentPosition;
            }
            if ((orientationVector.x > 0 && transform.localScale.x * transformMultiplier < 0) ||
                (orientationVector.x < 0 && transform.localScale.x * transformMultiplier > 0))
            {
                // We are not looking the way we should be looking! Flip the sprite.
                transform.localScale = new Vector3(transform.localScale.x * -1 * transformMultiplier, transform.localScale.y, transform.localScale.z);
            }
        }
    }
}
