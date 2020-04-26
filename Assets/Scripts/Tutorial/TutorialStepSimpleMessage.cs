using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.UI;
using GeneralAlgorithms.Algorithms.Common;
using UnityEngine;

namespace Assets.Scripts.Tutorial
{
    public class TutorialStepSimpleMessage: TutorialStep
    {
        public string Title;

        [TextArea(3, 6)]
        public string Message;

        private bool isOver;
        private MessageBox messageBox;
        private bool didMessageBoxAppear;

        private void Start()
        {
            messageBox = GetComponentInChildren<MessageBox>();
            messageBox.Appeared += MessageBox_Appeared;
            messageBox.Disappeared += MessageBox_Disappeared;
            messageBox.Show(Title, Message);
        }

        private void Update()
        {
            if (didMessageBoxAppear && UnityEngine.Input.anyKeyDown)
            {
                messageBox.Hide();
            }
        }

        private void MessageBox_Disappeared(object sender, EventArgs e)
        {
            isOver = true;
        }

        private void MessageBox_Appeared(object sender, EventArgs e)
        {
            didMessageBoxAppear = true;
        }

        public override bool IsTutorialStepOver()
        {
            return isOver;
        }

        private void OnDestroy()
        {
            messageBox.Appeared -= MessageBox_Appeared;
            messageBox.Disappeared -= MessageBox_Disappeared;
        }
    }
}
