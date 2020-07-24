using System.Collections;
using Assets.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts.Effects
{
    /// <summary>
    /// An effects that is shown when someone uses a taunt or targeting skill.
    /// Is a circle that starts with radius <see cref="MinRadius"/> and linearly increases itself to <see cref="MaxRadius"/> over <see cref="AnimationLength"/> seconds.
    /// </summary>
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
        /// <summary>
        /// How much should the radius end per second.
        /// </summary>
        private float radiusChangePerSecond;
        /// <summary>
        /// The circle whose radius should increase during this animation.
        /// </summary>
        private Circle animatedCircle;

        private void Start()
        {
            animatedCircle = GetComponent<Circle>();
            radiusChangePerSecond = MaxRadius / AnimationLength;
        }
        /// <summary>
        /// Plays the effect this component represents.
        /// </summary>
        public void StartEffect()
        {
            animatedCircle.IsVisible = true;
            animatedCircle.Radius = MinRadius;
            StartCoroutine(TauntAnimation());
        }
        /// <summary>
        /// The coroutine which increases the radius of the circle until it reaches the specified size.
        /// Then the circle disappears.
        /// </summary>
        /// <returns>The enumerator representing this coroutine.</returns>
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
