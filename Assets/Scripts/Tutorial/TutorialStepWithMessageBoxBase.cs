using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts.Tutorial
{
    /// <summary>
    /// <inheritdoc/>
    /// This step will show a message box across the screen that will stay there until the player finishes some action.
    /// When he does, we will still wait for the message box to disappear. Only then will the step be over.
    /// </summary>
    public class TutorialStepWithMessageBoxBase: TutorialStep
    {
        /// <summary>
        /// Title of the message box shown in this tutorial.
        /// </summary>
        public string Title;
        /// <summary>
        /// Message shown in the tutorial.
        /// </summary>
        [TextArea(3, 6)]
        public string Message;
        /// <summary>
        /// If true, this step is completely over and we should move to the next step. The message box was hidden
        /// </summary>
        protected bool isOver;
        /// <summary>
        /// The message box which appears and shows some message to the player.
        /// </summary>
        protected MessageBox messageBox;
        /// <summary>
        /// If true, the message box finished its appear animation.
        /// </summary>
        protected bool didMessageBoxAppear;
        /// <summary>
        /// If true, the player finished the action he was supposed to do in this tutorial step.
        /// </summary>
        protected bool completedTutorialAction;
        /// <summary>
        /// Called before the first update method. Displays the message box for this tutorial step.
        /// </summary>
        protected virtual void Start()
        {
            messageBox = GetComponentInChildren<MessageBox>();
            messageBox.Appeared += MessageBox_Appeared;
            messageBox.Disappeared += MessageBox_Disappeared;
            messageBox.Show(Title, Message);
        }
        /// <summary>
        /// Called when the message box disappears. Ends this tutorial step unless overridden.
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Arguments of the event</param>
        protected virtual void MessageBox_Disappeared(object sender, EventArgs e)
        {
            isOver = true;
        }
        /// <summary>
        /// Called when the message box appears.
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Arguments of the event</param>
        protected virtual void MessageBox_Appeared(object sender, EventArgs e)
        {
            didMessageBoxAppear = true;
        }
        /// <summary>
        /// <inheritdoc/>
        /// This will return true once the player finishes the action and the message box disappears.
        /// </summary>
        /// <returns></returns>
        public override bool IsTutorialStepOver()
        {
            return isOver;
        }
        /// <summary>
        /// When destroyed, unsubscribe from events.
        /// </summary>
        protected virtual void OnDestroy()
        {
            messageBox.Appeared -= MessageBox_Appeared;
            messageBox.Disappeared -= MessageBox_Disappeared;
        }
    }
}
