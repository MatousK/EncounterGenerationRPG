using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class SurveyController: MonoBehaviour
    {
        public string SurveyUrl;
        public Button FillOutSurveyButton;
        public Button ContinueButton;

        public void OnSurveyButtonPressed()
        {
            Application.OpenURL(SurveyUrl);
            ContinueButton.interactable = true;
        }
    }
}
