using TMPro;
using UnityEngine;

namespace Assets.Scripts.UI.Skills
{
    /// <summary>
    /// Component for the widget which displays a name of a skill when the player has his cursor over the skill.
    /// </summary>
    public class SkillName: MonoBehaviour
    {
        /// <summary>
        /// Name of the skill that should be shown. Internal, do not use, use <see cref="Text"/>.
        /// </summary>
        private string text;
        /// <summary>
        /// Name of the skill that should be shown.
        /// </summary>
        public string Text
        {
            get => text;
            set
            {
                text = value;
                GetComponentInChildren<TextMeshProUGUI>().text = text;
            }
        }
        /// <summary>
        /// If true, this widget should be visible right now. Internal, do not use, use <see cref="IsVisible"/>.
        /// </summary>
        private bool isVisible;
        /// <summary>
        /// If true, this widget should be visible right now. Plays the appropriate hide and show animations.
        /// </summary>
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
        /// <summary>
        /// Plays the appear animation of this UI element.
        /// </summary>
        private void Appear()
        {
            GetComponent<Animator>().SetBool("IsVisible", true);
        }
        /// <summary>
        /// Plays the disappear animation of this UI element.
        /// </summary>
        private void Disappear()
        {
            GetComponent<Animator>().SetBool("IsVisible", false);
        }
    }
}
