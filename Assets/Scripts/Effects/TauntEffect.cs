using System.Collections;
using Assets.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts.Effects
{
    public class TauntEffect : MonoBehaviour
    {
        /// <summary>
        /// Minimum size of the circle at the beginning of the animation.
        /// </summary>
        public float MinRadius = 0;
        /// <summary>
        /// Maximum radius of the circle at the end of the animation.
        /// </summary>
        public float MaxRadius = 100;
        /// <summary>
        /// How long should the animation be.
        /// </summary>
        public float AnimationLength = 1;
        private float radiusChangePerSecond;
        private Circle animatedCircle;

        private void Start()
        {
            animatedCircle = GetComponent<Circle>();
            radiusChangePerSecond = MaxRadius / AnimationLength;
        }

        public void StartEffect()
        {
            animatedCircle.IsVisible = true;
            animatedCircle.Radius = MinRadius;
            StartCoroutine(TauntAnimation());
        }

        private IEnumerator TauntAnimation()
        {
            while (animatedCircle.Radius < MaxRadius)
            {
                animatedCircle.Radius += Time.deltaTime * radiusChangePerSecond;
                yield return null;
            }
            animatedCircle.IsVisible = false;
        }
    }
}
