using Assets.Scripts.UI.Credits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class SkipButton: MonoBehaviour
    {
        public bool isCredits;
        private TypewriterText controlledTypewriterText;
        private void Start()
        {
            if (!isCredits)
            {
                controlledTypewriterText = FindObjectOfType<TypewriterText>();
                controlledTypewriterText.TextAnimationDone += ControlledTypewriterText_TextAnimationDone;
            }
        }

        private void Update()
        {
            if (UnityEngine.Input.GetKeyUp(KeyCode.Return))
            {
                if (isCredits)
                {
                    FindObjectOfType<CreditsController>().CreditsOver();
                } else
                {
                    controlledTypewriterText.FinishAnimation();
                }
            }
            else if (UnityEngine.Input.anyKeyDown)
            {
                GetComponent<Animation>().Play();
            }
        }

        private void ControlledTypewriterText_TextAnimationDone(object sender, EventArgs e)
        {
            Destroy(gameObject);
        }
    }
}
