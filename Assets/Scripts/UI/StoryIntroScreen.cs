using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.GameFlow;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class StoryIntroScreen: MonoBehaviour
    {
        public TypewriterText IntroTypewriterText;
        public GameObject SurveyButtons;

        public void Appear()
        {
            gameObject.SetActive(true);
            IntroTypewriterText.TextAnimationDone += IntroTypewriterText_TextAnimationDone;
        }

        private void IntroTypewriterText_TextAnimationDone(object sender, EventArgs e)
        {
            SurveyButtons.SetActive(true);
        }

        public void IntroDone()
        {
            FindObjectOfType<LevelLoader>().StartStoryMode();
        }
    }
}
