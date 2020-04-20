using UnityEngine;

namespace Assets.Scripts.Movement
{
    public class VelocityManager : MonoBehaviour
    {
        private Vector2? previousFramePosition = null;
        private Vector2 velocity = Vector2.zero;
        // Start is called before the first frame update
        void Start()
        {
            previousFramePosition = transform.position;
        }

        // We use late update instead of regular update to allow the movement of actor to occur before calculating velocity.
        void LateUpdate()
        {
            Vector2 currentPosition = transform.position;
            if (previousFramePosition.HasValue)
            {
                velocity = currentPosition - previousFramePosition.Value;
            }
            else
            {
                Debug.Assert(false, "Previous frame value should be set in start, so it should never be null.");
            }
            previousFramePosition = transform.position;
        }

        public Vector2 GetVelocity()
        {
            return velocity;
        }
    }
}
