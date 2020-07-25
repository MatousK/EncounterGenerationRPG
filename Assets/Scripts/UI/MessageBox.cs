using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// A component for a UI element which is like a message box, i.e. can show a box across the screen with some title and text.
    /// </summary>
    public class MessageBox: MonoBehaviour
    {
        /// <summary>
        /// The field which should show the message box label.
        /// </summary>
        public TextMeshProUGUI TitleLabel;
        /// <summary>
        /// The field which should show the message box text.
        /// </summary>
        public TextMeshProUGUI DescriptionLabel;
        /// <summary>
        /// Event raised when the message box finishes its appear animation.
        /// </summary>
        public event EventHandler Appeared;
        /// <summary>
        /// Event raised when the message box finishes its disappear animation.
        /// </summary>
        public event EventHandler Disappeared;
        /// <summary>
        /// Displays the message box.
        /// </summary>
        /// <param name="title">Title of the message box.</param>
        /// <param name="message">Text in the message box.</param>
        public void Show(string title, string message)
        {
            TitleLabel.text = title;
            DescriptionLabel.text = message;
            GetComponent<Animation>().PlayQueued("MessageBoxAppear");
        }
        /// <summary>
        /// Hides the message box.
        /// </summary>
        public void Hide()
        {
            GetComponent<Animation>().PlayQueued("MessageBoxDisappear");
        }
        /// <summary>
        /// Called from the animation when the appear animation finishes.
        /// </summary>
        public void OnAppeared()
        {
            Appeared?.Invoke(this, new EventArgs());
        }
        /// <summary>
        /// Called by the animation when the disappear animation finishes.
        /// </summary>
        public void OnDisappeared()
        {
            Disappeared?.Invoke(this, new EventArgs());
        }



    }
}
