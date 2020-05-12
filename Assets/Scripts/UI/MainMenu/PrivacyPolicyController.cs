using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.MainMenu
{
    class PrivacyPolicyController: MonoBehaviour
    {
        public Toggle AgreeToggle;
        public Button ContinueButton;

        private void Update()
        {
            ContinueButton.interactable = AgreeToggle.isOn;
        }
    }
}
