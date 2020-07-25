using UnityEngine;

namespace Assets.Scripts.Movement
{
    /// <summary>
    /// Calculates the velocity of the character, i.e. how has his position changed since the last frame.
    /// </summary>
    public class VelocityManager : MonoBehaviour
    {
        /// <summary>
        /// The position where we were last frame.
        /// </summary>
        private Vector2? previousFramePosition = null;
        /// <summary>
        /// The current velocity.
        /// </summary>
        private Vector2 velocity = Vector2.zero;
        // Start is called before the first frame update
        void Start()
        {
            previousFramePosition = transform.position;
        }

        /// <summary>
        /// Late update is executed after all other update methods.
        /// We use late update instead of regular update to allow the movement of actor to occur before calculating velocity.
        /// </summary>
        void LateUpdate()
        {
            Vector2 currentPosition = transform.position;
            if (previousFramePosition.HasValue)
            {
                velocity = currentPosition - previousFramePosition.Value;
            }
            else
            {
                UnityEngine.Debug.Assert(false, "Previous frame value should be set in start, so it should never be null.");
            }
            previousFramePosition = transform.position;
        }
        /// <summary>
        /// Retrieves the current velocity..
        /// </summary>
        /// <returns>The velocity of the character.</returns>
        public Vector2 GetVelocity()
        {
            return velocity;
        }
    }
}
