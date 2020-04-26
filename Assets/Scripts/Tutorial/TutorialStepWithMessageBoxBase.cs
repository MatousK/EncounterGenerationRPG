using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts.Tutorial
{
    public class TutorialStepWithMessageBoxBase: TutorialStep
    {
        public string Title;

        [TextArea(3, 6)]
        public string Message;

        protected bool isOver;
        protected MessageBox messageBox;
        protected bool didMessageBoxAppear;
        protected bool completedTutorialAction;

        protected virtual void Start()
        {
            messageBox = GetComponentInChildren<MessageBox>();
            messageBox.Appeared += MessageBox_Appeared;
            messageBox.Disappeared += MessageBox_Disappeared;
            messageBox.Show(Title, Message);
        }

        protected virtual void MessageBox_Disappeared(object sender, EventArgs e)
        {
            isOver = true;
        }

        protected virtual void MessageBox_Appeared(object sender, EventArgs e)
        {
            didMessageBoxAppear = true;
        }

        public override bool IsTutorialStepOver()
        {
            return isOver;
        }

        protected virtual void OnDestroy()
        {
            messageBox.Appeared -= MessageBox_Appeared;
            messageBox.Disappeared -= MessageBox_Disappeared;
        }
    }
}
