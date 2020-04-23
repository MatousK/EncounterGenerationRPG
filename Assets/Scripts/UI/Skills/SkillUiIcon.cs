using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Combat.Skills;
using Assets.Scripts.Input;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Skills
{
    public class SkillUiIcon: MonoBehaviour
    {
        public bool IsFriendlySkill;

        public Image SkillIcon;

        public Image Frame;

        public Image CooldownOverlay;

        public float CurrentCooldownPercentage;

        private Skill representedSkill;

        private Button buttonComponent;

        private SkillFromUiIconClickController skillFromUiIconClickController;

        private void Start()
        {
            buttonComponent = GetComponent<Button>();
            skillFromUiIconClickController = FindObjectOfType<SkillFromUiIconClickController>();
        }

        private void Update()
        {
            CooldownOverlay.transform.localScale = new Vector3(1, CurrentCooldownPercentage,1);
            buttonComponent.interactable = CurrentCooldownPercentage == 0;
            var isTargetingRepresentedSkill = skillFromUiIconClickController.TargetedSkill == representedSkill;
            var isSelected = EventSystem.current.currentSelectedGameObject == gameObject;
            if (isTargetingRepresentedSkill && !isSelected)
            {
                // This skill is being used, but we lost focus - we probably clicked on something else.
                skillFromUiIconClickController.TryUseSkillOnCombatantUnderCursor();
                skillFromUiIconClickController.TargetedSkill = null;
            }
        }

        public void SetSkill(Skill skill, Sprite frameSprite)
        {
            representedSkill = skill;

            SkillIcon.sprite = skill.SkillIcon;

            Frame.sprite = frameSprite;
        }

        public void OnSkillPressed()
        {
            if (representedSkill is PersonalSkill personalSkill)
            {
                personalSkill.ActivateSkill();
            }
            else if (representedSkill is TargetedSkill targetedSkill)
            {
                skillFromUiIconClickController.IsFriendlySkill = IsFriendlySkill;
                skillFromUiIconClickController.TargetedSkill = targetedSkill;
            }
            else
            {
                UnityEngine.Debug.Assert(false, "Unknown skill");
            }
        }
    }
}
