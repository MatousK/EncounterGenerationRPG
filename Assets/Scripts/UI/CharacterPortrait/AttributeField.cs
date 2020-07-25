using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.CharacterPortrait
{
    /// <summary>
    /// A field showing an attribute of a character in the UI.
    /// </summary>
    public class AttributeField: MonoBehaviour
    {
        /// <summary>
        /// Value shown in this field in the last frame. Null if this is the first frame.
        /// We need it to detect attribute increase.
        /// </summary>
        private float? lastFrameValueToShow;
        /// <summary>
        /// The value we should show in this field in this frame.
        /// </summary>
        public float ValueToShow;
        /// <summary>
        /// The field into which we should write the value of the attribute.
        /// </summary>
        public Text Label;
        /// <summary>
        /// Called before the first Update. Sets the Text value.
        /// </summary>
        public void Start()
        {
            UpdateLabel();
        }
        /// <summary>
        /// Call every update. Set the Text value.
        /// If the attribute increased, show the animation which draws attention to that, so the player knows that yes, there was an attribute increase.
        /// </summary>
        public void Update()
        {
            UpdateLabel();
            if (lastFrameValueToShow.HasValue && lastFrameValueToShow < ValueToShow)
            {
                GetComponent<Animation>().Play();
            }
            lastFrameValueToShow = ValueToShow;
        }
        /// <summary>
        /// Sets the value to the label to correctly represent the field.
        /// </summary>
        void UpdateLabel()
        {
            Label.text = ValueToShow.ToString();
        }
    }
}