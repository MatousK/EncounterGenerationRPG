using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Combat.Skills;
using Assets.Scripts.Cutscenes;
using Assets.Scripts.Input;
using Assets.Scripts.Sound.CharacterSounds;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Skills
{
    /// <summary>
    /// An icon representing a skill in the UI, shown under the portrait.
    /// Shows the icon, shows name when mouse over, shows description on right clicks and allows the skill to be used by clicking on it.
    /// </summary>
    public class SkillUiIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        /// <summary>
        /// How long must the cursor be over the skill for the name to appear in seconds.
        /// </summary>
        public float SkillNameAppearDelay = 0.5f;
        /// <summary>
        /// If true, the represented skill is a friendly skill.
        /// </summary>
        public bool IsFriendlySkill;
        /// <summary>
        /// The UI element which can display the name of this skill.
        /// </summary>
        public SkillName SkillNameWidget;
        /// <summary>
        /// The overlay which should be used if the skill is being used, by this we mean that he left clicked on it an is selecting the target.
        /// </summary>
        public Image IsBeingUsedOverlay;
        /// <summary>
        /// The UI element which should contain the image of this skill.
        /// </summary>
        public Image SkillIcon;
        /// <summary>
        /// UI element which should show the border around the image.
        /// </summary>
        public Image Frame;
        /// <summary>
        /// The overlay over this image which should indicate how much longer will the skill be in cooldown. 
        /// Will be scaled on the Y coordinate to indicate the time remaining of the cooldown.
        /// </summary>
        public Image CooldownOverlay;
        /// <summary>
        /// How many percent of the cooldown are remaining and should be shown.
        /// </summary>
        public float CurrentCooldownPercentage;
        /// <summary>
        /// The skill this icon represents.
        /// </summary>
        private Skill representedSkill;
        /// <summary>
        /// The button used for detecting clicks and showing mouse over effect.
        /// </summary>
        private Button buttonComponent;
        /// <summary>
        /// The component which controls which skills (if any) is currently being used from UI.
        /// When clicking this skill we must set the skill used on this object.
        /// </summary>
        private SkillFromUiIconClickController skillFromUiIconClickController;
        /// <summary>
        /// The component which knows whether we are in a cutscene.
        /// We disable skill usage in cutscenes.
        /// </summary>
        private CutsceneManager cutsceneManager;
        /// <summary>
        /// While the pointer is over this element, this is the time when it entered. When not over element, this is null.
        /// </summary>
        private float? pointerEnterTime;
        /// <summary>
        /// Called before first update. Finds references to all dependencies.
        /// </summary>
        private void Start()
        {
            buttonComponent = GetComponent<Button>();
            skillFromUiIconClickController = FindObjectOfType<SkillFromUiIconClickController>();
            cutsceneManager = FindObjectOfType<CutsceneManager>();
        }
        /// <summary>
        /// Called every frame. Updates the state of indicators on the skill.
        /// Also shows the name if the mouse was over this element for sufficient time.
        /// </summary>
        private void Update()
        {
            CooldownOverlay.transform.localScale = new Vector3(1, CurrentCooldownPercentage, 1);
            buttonComponent.interactable = CurrentCooldownPercentage == 0 && !cutsceneManager.IsCutsceneActive;
            var isBeingUsed = skillFromUiIconClickController.TargetedSkill == representedSkill;
            IsBeingUsedOverlay.enabled = isBeingUsed;

            if (pointerEnterTime != null && Time.unscaledTime - pointerEnterTime.Value > SkillNameAppearDelay)
            {
                SkillNameWidget.IsVisible = true;
            }
        }
        /// <summary>
        /// Sets the skill this component will represent.
        /// </summary>
        /// <param name="skill">The skill we will represent.</param>
        /// <param name="frameSprite">The image that should be used for the border of this skill.</param>
        public void SetSkill(Skill skill, Sprite frameSprite)
        {
            representedSkill = skill;

            SkillIcon.sprite = skill.SkillIcon;

            Frame.sprite = frameSprite;

            SkillNameWidget.Text = skill.SkillName;
        }
        /// <summary>
        /// Called when the user presses the skill.
        /// Will start using this skill if it is a personal skill or instruct the <see cref="skillFromUiIconClickController"/> that the user is trying to use this skill.
        /// </summary>
        public void OnSkillPressed()
        {
            var skillUsingHero = GetComponentInParent<HeroSkillsContainer>().RepresentedHero;
            if (representedSkill is PersonalSkill)
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
        /// <summary>
        /// Called when the pointer enters this skill icon. Will start the timer that will eventually show the skill name.
        /// Called automatically by Unity.
        /// </summary>
        /// <param name="eventData">The data about the pointer which entered the icon.</param>
        public void OnPointerEnter(PointerEventData eventData)
        {
            pointerEnterTime = Time.unscaledTime;
        }
        /// <summary>
        /// Called when the pointer leaves this skill icon. Will stop the timer that might have showed the skill name.
        /// If the skill name was shown, hide it.
        /// Called automatically by Unity.
        /// </summary>
        /// <param name="eventData">The data about the pointer which left the icon.</param>
        public void OnPointerExit(PointerEventData eventData)
        {
            pointerEnterTime = null;
            SkillNameWidget.IsVisible = false;
        }
        /// <summary>
        /// Called automatically by Unity. We only use this to detect right clicks, not left clicks, those are detected by this being a button.
        /// On right click, show the skill description overlay for the current skill.
        /// </summary>
        /// <param name="eventData">Info about the click which triggered the event.</param>
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                FindObjectOfType<SkillDescriptionOverlay>().Show(representedSkill.SkillName, representedSkill.SkillDescription,
                    representedSkill.Cooldown);
            }
        }
    }
}
