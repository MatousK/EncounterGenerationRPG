using UnityEngine;

namespace Assets.Scripts.UI
{
    class FloatingText: MonoBehaviour
    {
        void Update()
        {
            if (transform.parent.localScale.x < 0 != transform.localScale.x < 0)
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
        }

        public void OnDisappearAnimationFinished()
        {
            // Text animation has finished, we should destroy this.
            Destroy(gameObject);
        }
    }
}
