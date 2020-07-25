using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Input;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.UI.Skills
{
    /// <summary>
    /// Component controlling the overlay that appears when the user opens a skill description.
    /// The game is paused while this overlay is active.
    /// </summary>
    public class SkillDescriptionOverlay: MonoBehaviour
    {
        /// <summary>
        /// If true, this overlay is currently visible.
        /// </summary>
        private bool isVisible;
        /// <summary>
        /// The widget showing the title of the skill shown in the overlay.
        /// </summary>
        public TextMeshProUGUI SkillTitle;
        /// <summary>
        /// The widget showing the description of the skill shown in the overlay.
        /// </summary>
        public TextMeshProUGUI SkillDescription;
        /// <summary>
        /// The widget showing the cooldown of the skill shown in the overlay.
        /// </summary>
        public TextMeshProUGUI CooldownText;
        /// <summary>
        /// The widget containing all information about the skill.
        /// </summary>
        public GameObject SkillDescriptionWidget;
        /// <summary>
        /// Called every frame. Hides this overlay if the user clicks on anything and this widget is visible.
        /// </summary>
        private void Update()
        {
            if (isVisible && UnityEngine.Input.anyKeyDown)
            {
                Hide();
            }
        }
        /// <summary>
        /// Shows the given description of some skill.
        /// </summary>
        /// <param name="skillName">Name of the skill.</param>
        /// <param name="skillDescription">Description of the skill.</param>
        /// <param name="cooldown">How long is the cooldown of the skill.</param>
        public void Show(string skillName, string skillDescription, float cooldown)
        {
            SkillTitle.text = skillName;
            SkillDescription.text = skillDescription;
            SkillDescriptionWidget.SetActive(true);
            int cooldownRounded = (int) (cooldown);
            CooldownText.text = $"Cooldown: {cooldownRounded} seconds";
            isVisible = true;
            FindObjectOfType<PauseManager>().IsPausedByUi = true;
        }
        /// <summary>
        /// Hides this overlay.
        /// </summary>
        public void Hide()
        {
            SkillDescriptionWidget.SetActive(false);
            isVisible = false;
            FindObjectOfType<PauseManager>().IsPausedByUi = false;
        }
    }
}
