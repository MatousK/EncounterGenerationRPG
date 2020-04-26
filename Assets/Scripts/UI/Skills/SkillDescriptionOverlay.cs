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
    public class SkillDescriptionOverlay: MonoBehaviour
    {
        private bool isVisible;

        public TextMeshProUGUI SkillTitle;
        public TextMeshProUGUI SkillDescription;
        public TextMeshProUGUI CooldownText;
        public GameObject SkillDescriptionWidget;

        private void Update()
        {
            if (isVisible && UnityEngine.Input.anyKeyDown)
            {
                Hide();
            }
        }

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

        public void Hide()
        {
            SkillDescriptionWidget.SetActive(false);
            isVisible = false;
            FindObjectOfType<PauseManager>().IsPausedByUi = false;
        }
    }
}
