using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Combat.Skills;
using Assets.Scripts.Input;
using Assets.Scripts.Sound.CharacterSounds;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Skills
{
    public class SkillUiIcon: MonoBehaviour
    {
        public bool IsFriendlySkill;

        public Image IsBeingUsedOverlay;

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
            var isBeingUsed = skillFromUiIconClickController.TargetedSkill == representedSkill;
            IsBeingUsedOverlay.enabled = isBeingUsed;
        }

        public void SetSkill(Skill skill, Sprite frameSprite)
        {
            representedSkill = skill;

            SkillIcon.sprite = skill.SkillIcon;

            Frame.sprite = frameSprite;
        }

        public void OnSkillPressed()
        {
            var skillUsingHero = GetComponentInParent<HeroSkillsContainer>().RepresentedHero;
            if (representedSkill is PersonalSkill personalSkill)
            {
                skillUsingHero.SelfSkillUsed();
                var speakingCharacterVoice = skillUsingHero.GetComponentInChildren<CharacterVoiceController>();
                if (speakingCharacterVoice != null)
                {
                    speakingCharacterVoice.OnOrderGiven(VoiceOrderType.SelfSkill);
                }
            }
            else if (representedSkill is TargetedSkill targetedSkill)
            {
                skillFromUiIconClickController.IsFriendlySkill = IsFriendlySkill;
                skillFromUiIconClickController.SetUsedSkill(skillUsingHero, targetedSkill);
            }
            else
            {
                UnityEngine.Debug.Assert(false, "Unknown skill");
            }
        }
    }
}
