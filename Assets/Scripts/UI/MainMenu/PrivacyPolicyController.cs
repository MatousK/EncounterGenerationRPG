using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.MainMenu
{
    /// <summary>
    /// Component which ensures that the people can continue from privacy agreement only if he checks the checkbox.
    /// </summary>
    class PrivacyPolicyController: MonoBehaviour
    {
        /// <summary>
        /// The checkbox the user must toggle on to continue with the experiment.
        /// </summary>
        public Toggle AgreeToggle;
        /// <summary>
        /// The button to continue with the experiment. It should be enabled only if the <see cref="AgreeToggle"/> is checked.
        /// </summary>
        public Button ContinueButton;
        /// <summary>
        /// Called every frame. Updates whether the <see cref="ContinueButton"/> is clickable based on the value in the <see cref="AgreeToggle"/>
        /// </summary>
        private void Update()
        {
            ContinueButton.interactable = AgreeToggle.isOn;
        }
    }
}
