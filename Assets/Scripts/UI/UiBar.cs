using UnityEngine;

// Scales the attached object based on a percentage. Used for health bars above combatants.
namespace Assets.Scripts.UI
{
    public class UiBar : MonoBehaviour
    {
        // The percentage this bar is supposed to be showing
        public float Percentage;

        private void Start()
        {
            UpdateBarScaleAndPosition();
        }

        // Update is called once per frame
        void Update()
        {
            UpdateBarScaleAndPosition();
        }

        void UpdateBarScaleAndPosition()
        {
            // Scale should match the percentage, position should move left a bit so the health is still left aligned
            transform.localScale = new Vector3(Percentage, transform.localScale.y, transform.localScale.z);
            transform.localPosition = new Vector3(-0.5f + Percentage / 2, transform.localPosition.y, transform.localPosition.z);
        }
    }
}
