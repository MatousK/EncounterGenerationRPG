using UnityEngine;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// Scales the game object the component is attached to based on <see cref="Percentage"/>. Used for health and cooldown bars above and under combatants.
    /// </summary>
    public class UiBar : MonoBehaviour
    {
        /// <summary>
        /// The percentage this bar is supposed to be showing
        /// </summary>
        public float Percentage;
        /// <summary>
        /// Called before first update frame. Updates the scale to match <see cref="Percentage"/>.
        /// </summary>
        private void Start()
        {
            UpdateBarScaleAndPosition();
        }

        /// <summary>
        /// Called every frame. Updates the scale to match <see cref="Percentage"/>.
        /// </summary>
        void Update()
        {
            UpdateBarScaleAndPosition();
        }
        /// <summary>
        /// Updates the scale to match <see cref="Percentage"/>.
        /// </summary>
        void UpdateBarScaleAndPosition()
        {
            // Scale should match the percentage, position should move left a bit so the health or cooldown is still left aligned
            transform.localScale = new Vector3(Percentage, transform.localScale.y, transform.localScale.z);
            transform.localPosition = new Vector3(-0.5f + Percentage / 2, transform.localPosition.y, transform.localPosition.z);
        }
    }
}
