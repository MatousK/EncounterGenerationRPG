using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Effects
{
    /// <summary>
    /// Helper class for effects that will destroy the game object it is attached to <see cref="AutoDestroyTimeSeconds"/> seconds after activation.
    /// </summary>
    public class AutoDestroyEffect: MonoBehaviour
    {
        /// <summary>
        /// Time after which this effect should destroy itself.
        /// </summary>
        public float AutoDestroyTimeSeconds = 1;

        private void Start()
        {
            StartCoroutine(WaitAndDestroy());
        }
        /// <summary>
        /// Wait for <see cref="AutoDestroyTimeSeconds"/> seconds and then destroy the object this component is attached to.
        /// </summary>
        /// <returns>The enumerator of this coroutine.</returns>
        private IEnumerator WaitAndDestroy()
        {
            yield return new WaitForSeconds(AutoDestroyTimeSeconds);
            Destroy(gameObject);
        }
    }
}