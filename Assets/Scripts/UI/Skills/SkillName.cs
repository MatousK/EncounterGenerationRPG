using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.UI.Skills
{
    public class SkillName: MonoBehaviour
    {
        private string text;
        public string Text
        {
            get => text;
            set
            {
                text = value;
                GetComponentInChildren<TextMeshProUGUI>().text = text;
            }
        }

        private bool isVisible;
        public bool IsVisible
        {
            get => isVisible;
            set
            {
                if (isVisible != value)
                {
                    isVisible = value;
                    if (isVisible)
                    {
                        Appear();
                    }
                    else
                    {
                        Disappear();
                    }
                }
            }
        }

        private void Start()
        {
        }

        private void Appear()
        {
            GetComponent<Animator>().SetBool("IsVisible", true);
        }

        private void Disappear()
        {
            GetComponent<Animator>().SetBool("IsVisible", false);
        }
    }
}
