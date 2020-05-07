using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.CharacterPortrait
{
    public class AttributeField: MonoBehaviour
    {
        public float? lastFrameValueToShow;
        public float ValueToShow;
        public Text Label;

        public void Start()
        {
            UpdateLabel();
        }

        public void Update()
        {
            UpdateLabel();
            if (lastFrameValueToShow.HasValue && lastFrameValueToShow < ValueToShow)
            {
                GetComponent<Animation>().Play();
            }
            lastFrameValueToShow = ValueToShow;
        }

        void UpdateLabel()
        {
            Label.text = ValueToShow.ToString();
        }
    }
}