using System.Linq;
using Assets.Scripts.Camera;
using Assets.Scripts.Combat;
using Assets.Scripts.Combat.Conditions;
using Assets.Scripts.Input;
using Assets.Scripts.Sound.CharacterSounds;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.UI.CharacterPortrait
{
    /// <summary>
    /// Controls the character's. Everything regarding it - background, indicators, attributes, clicking on the portrait everything.
    /// </summary>
    public class CharacterPortrait : MonoBehaviour, IPointerClickHandler
    {
        /// <summary>
        /// The component which knows about all combatants in the game.
        /// </summary>
        CombatantsManager combatantsManager;
        /// <summary>
        /// The hero this portrait represents.
        /// </summary>
        public Hero RepresentedHero;
        /// <summary>
        /// The field where we should show the attack strength of the hero.
        /// </summary>
        public AttributeField AttackField;
        /// <summary>
        /// The field where we should show the maximum health of a hero.
        /// </summary>
        public AttributeField MaxHealthField;
        /// <summary>
        /// Gradient specifying how should the character's background change as he gets more hurt. 
        /// </summary>
        public Gradient PortraitBackgroundGradient;
        /// <summary>
        /// The image which is the background of the image. It should have the color controlled by character's health an d<see cref="PortraitBackgroundGradient"/>.
        /// </summary>
        public Image ImageBackground;
        /// <summary>
        /// The portrait of the hero.
        /// </summary>
        public Image PortraitImage;
        /// <summary>
        /// Indicator showing the hero's max health.
        /// </summary>
        public Image MaxHealthIndicator;
        /// <summary>
        /// Indicator showing the hero's current health.
        /// </summary>
        public Image CurrentHealthIndicator;
        /// <summary>
        /// A border around the hero. Colored green if the hero is selected.
        /// </summary>
        public Image Border;
        /// <summary>
        /// An icon that appears if the hero is targeted.
        /// </summary>
        public TargetedIndicatorIcon TargetedIndicator;

        /// <summary>
        /// When was the last time we clicked on the portrait.
        /// Used to detect doubleClick.
        /// </summary>
        private float lastPortraitClickTime;
        /// <summary>
        /// How quickly after another must clicks happen to be considered a double click.
        /// Used to detect doubleClick.
        /// </summary>
        private const float DoubleClickTime = 0.25f;
        /// <summary>
        /// Component that can move the camera around, used to quickly pan to hero on double click on portrait.
        /// </summary>
        private CameraMovement cameraMovement;
        /// <summary>
        /// The component which can play the voices of the character.
        /// </summary>
        private CharacterVoiceController heroVoiceController;
        /// <summary>
        /// Class used to handle usage of skills from UI.
        /// When the player clicks on a skill, we let this component know that the player is using that skill from UI.
        /// </summary>
        private SkillFromUiIconClickController skillFromUiIconClickController;
        /// <summary>
        /// Called before the first update. Finds references to dependencies.
        /// </summary>
        void Start()
        {
            combatantsManager = FindObjectOfType<CombatantsManager>();
            cameraMovement = FindObjectOfType<CameraMovement>();
            skillFromUiIconClickController = FindObjectOfType<SkillFromUiIconClickController>();
        }
        /// <summary>
        /// Called every frame. Update all the properties to match the hero's state.
        /// </summary>
        public void Update()
        {
            if (RepresentedHero == null)
            {
                heroVoiceController = null;
                return;
            }

            heroVoiceController = RepresentedHero.GetComponentInChildren<CharacterVoiceController>();
            PortraitImage.sprite = RepresentedHero.Portrait;

            var dps = (int)(RepresentedHero.Attributes.DealtDamageMultiplier * RepresentedHero.Attributes.AttackSpeedMultiplier);
            AttackField.ValueToShow = dps;
            MaxHealthField.ValueToShow = (int)RepresentedHero.TotalMaxHitpoints;

            var maxHealthPercentage = RepresentedHero.MaxHitpoints / RepresentedHero.TotalMaxHitpoints;
            var currentHealthPercentage = RepresentedHero.HitPoints / RepresentedHero.TotalMaxHitpoints;
            var totalHealthPercentage = (currentHealthPercentage + maxHealthPercentage) / 2;

            ImageBackground.color = PortraitBackgroundGradient.Evaluate(1 - totalHealthPercentage);

            MaxHealthIndicator.rectTransform.anchorMax = new Vector2(maxHealthPercentage, MaxHealthIndicator.rectTransform.anchorMax.y);

            CurrentHealthIndicator.rectTransform.anchorMax = new Vector2(currentHealthPercentage, CurrentHealthIndicator.rectTransform.anchorMax.y);

            var isSelected = RepresentedHero.GetComponent<SelectableObject>().IsSelected;
            Border.color = isSelected ? Color.green : Color.white;
            TargetedIndicator.RepresentedHero = RepresentedHero;
        }
        /// <summary>
        ///  Handle click on a hero. This can be both a left click right click, which can select a hero or give a command.
        /// </summary>
        /// <param name="eventData">Data about the click.</param>
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                skillFromUiIconClickController.ClearUsedSkill();
                if (Time.realtimeSinceStartup - lastPortraitClickTime < DoubleClickTime)
                {
                    // Double click detected.
                    cameraMovement.QuickFindHero(RepresentedHero);
                }
                else
                {
                    lastPortraitClickTime = Time.realtimeSinceStartup;
                    heroVoiceController.PlayOnSelectedSound();
                    SelectHero();
                }
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                if (skillFromUiIconClickController.IsUsingSkill)
                {
                    HandleSkillUsageFromUi();
                    skillFromUiIconClickController.ClearUsedSkill();
                }
                else
                {
                    DoActionOnHero();
                }
            }
        }
        /// <summary>
        /// Selects the represented hero.
        /// </summary>
        private void SelectHero()
        {
            RepresentedHero.GetComponent<SelectableObject>().IsSelected = true;
            if (!UnityEngine.Input.GetKey(KeyCode.LeftShift) && !UnityEngine.Input.GetKey(KeyCode.RightShift))
            {
                // If not holding shift, deselect, other characters.
                foreach (var playerCharacter in combatantsManager.PlayerCharacters)
                {
                    if (playerCharacter != RepresentedHero)
                    {
                        playerCharacter.GetComponent<SelectableObject>().IsSelected = false;
                    }
                }
            }
        }
        /// <summary>
        /// We right clicked on a hero, which some command. Give the command to the selected heroes. If appropriate, play the skill.
        /// </summary>
        private void DoActionOnHero()
        {
            var usingSkill = UnityEngine.Input.GetKey(KeyCode.LeftControl) || UnityEngine.Input.GetKey(KeyCode.RightControl);
            var selectedCharacters = combatantsManager.GetPlayerCharacters(onlySelected: true).ToList();
            var speakingHero = selectedCharacters.FirstOrDefault();
            var speakingCharacterVoice = speakingHero != null
                ? speakingHero.GetComponentInChildren<CharacterVoiceController>()
                : null;
            VoiceOrderType? voiceOrderType = null;
            // Go through each selected character and give him the order.
            foreach (var hero in selectedCharacters)
            {
                if (hero == RepresentedHero)
                {
                    if (usingSkill)
                    {
                        if (hero == speakingHero)
                        {
                            voiceOrderType = VoiceOrderType.SelfSkill;
                        }
                        hero.SelfSkillUsed();
                    }
                    else
                    {
                        hero.SelfClicked();
                    }
                }
                else
                {
                    if (usingSkill)
                    {
                        if (hero == speakingHero)
                        {
                            voiceOrderType = VoiceOrderType.FriendlySkill;
                        }
                        hero.FriendlySkillUsed(RepresentedHero);
                    }
                    else
                    {
                        hero.FriendlyClicked(RepresentedHero);
                    }
                }
            }

            if (voiceOrderType != null && speakingCharacterVoice != null)
            {
                speakingCharacterVoice.OnOrderGiven(voiceOrderType.Value);
            }
        }
        /// <summary>
        /// Called when the player right clicks on a player while having a skill selected.
        /// If the target is valid, i.e. this is a friendly skill and we are not targetting ourselves, execute it.
        /// </summary>
        private void HandleSkillUsageFromUi()
        {
            if (!skillFromUiIconClickController.IsFriendlySkill || RepresentedHero == skillFromUiIconClickController.CastingHero)
            {
                // Portrait can represent only a hero. So no chance of clicking on a monster, so only friendly skill allowed.
                return;
            }

            skillFromUiIconClickController.CastingHero.FriendlySkillUsed(RepresentedHero);
            var voiceController = skillFromUiIconClickController.CastingHero.GetComponentInChildren<CharacterVoiceController>();
            voiceController.OnOrderGiven(VoiceOrderType.FriendlySkill);
        }
    }
}