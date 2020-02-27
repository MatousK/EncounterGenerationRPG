using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterPortrait : MonoBehaviour, IPointerClickHandler
{
    CombatantsManager combatantsManager;

    public Hero RepresentedHero;

    public AttributeField AttackField;
    public AttributeField DefenseField;
    public AttributeField MaxHealthField;

    public Gradient PortraitBackgroundGradient;
    public Image ImageBackground;
    public Image PortraitImage;
    public Image MaxHealthIndicator;
    public Image CurrentHealthIndicator;
    public Image Border;

    // Used to detect doubleClick
    private float lastPortraitClickTime;
    private const float doubleClickTime = 0.25f;

    private CameraMovement cameraMovement;

    void Awake()
    {
        combatantsManager = FindObjectOfType<CombatantsManager>();
        cameraMovement = FindObjectOfType<CameraMovement>();
    }

    public void Update()
    {
        if (RepresentedHero == null)
        {
            return;
        }

        PortraitImage.sprite = RepresentedHero.Portrait;

        AttackField.ValueToShow = (int)RepresentedHero.Attributes.DealtDamageMultiplier;
        DefenseField.ValueToShow = (int)((1 - RepresentedHero.Attributes.ReceivedDamageMultiplier) * 100);
        MaxHealthField.ValueToShow = (int)RepresentedHero.TotalMaxHitpoints;

        var maxHealthPercentage = RepresentedHero.MaxHitpoints / RepresentedHero.TotalMaxHitpoints;
        var currentHealthPercentage = RepresentedHero.HitPoints / RepresentedHero.TotalMaxHitpoints;
        var totalHealthPercentage = (currentHealthPercentage + maxHealthPercentage) / 2;

        ImageBackground.color = PortraitBackgroundGradient.Evaluate(1 - totalHealthPercentage);

        MaxHealthIndicator.rectTransform.anchorMax = new Vector2(maxHealthPercentage, MaxHealthIndicator.rectTransform.anchorMax.y);

        CurrentHealthIndicator.rectTransform.anchorMax = new Vector2(currentHealthPercentage, CurrentHealthIndicator.rectTransform.anchorMax.y);

        var isSelected = RepresentedHero.GetComponent<SelectableObject>().IsSelected;
        Border.color = isSelected ? Color.green : Color.white;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (Time.realtimeSinceStartup - lastPortraitClickTime < doubleClickTime)
            {
                // Double click detected.
                cameraMovement.QuickFindHero(RepresentedHero);
            }
            else
            {
                lastPortraitClickTime = Time.realtimeSinceStartup;
                SelectHero();
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            DoActionOnHero();
        }
    }

    void SelectHero()
    {
        RepresentedHero.GetComponent<SelectableObject>().IsSelected = true;
        if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
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

    void DoActionOnHero()
    {
        var usingSkill = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
        foreach (var hero in combatantsManager.GetPlayerCharacters(onlySelected: true))
        {
            if (hero == RepresentedHero)
            {
                if (usingSkill)
                {
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
                    hero.FriendlySkillUsed(RepresentedHero);
                }
                else
                {
                    hero.FriendlyClicked(RepresentedHero);
                }
            }
        }
    }
}