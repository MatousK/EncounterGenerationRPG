using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Effects
{
    /// <summary>
    /// Helper class for effects that will destroy the attached component X seconds after activation.
    /// </summary>
    public class AutoDestroyEffect: MonoBehaviour
    {
        public float AutoDestroyTimeSeconds = 1;

        public void Start()
        {
            StartCoroutine(WaitAndDestroy());
        }

        public IEnumerator WaitAndDestroy()
        {
            yield return new WaitForSeconds(AutoDestroyTimeSeconds);
            Destroy(gameObject);
        }
    }
}