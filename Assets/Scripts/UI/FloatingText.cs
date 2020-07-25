using UnityEngine;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// This component will be spawned on heal or taken damage and will float above the target.
    /// This component takes care of making sure the text is not flipped because of combatant being flipped and of destroying the text once it reaches its peak.
    /// </summary>
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
