using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.CharacterPortrait
{
    public class AttributeField: MonoBehaviour
    {
        public float ValueToShow;
        public Text Label;

        public void Start()
        {
            UpdateLabel();
        }

        public void Update()
        {
            UpdateLabel();
        }

        void UpdateLabel()
        {
            Label.text = ValueToShow.ToString();
        }
    }
}