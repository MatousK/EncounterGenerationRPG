using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Combat;
using Assets.Scripts.Cutscenes;
using Assets.Scripts.Movement;
using Assets.Scripts.Sound.CharacterSounds;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Input
{
    public class RightClickController : MonoBehaviour
    {
        CombatantsManager combatantsManager;
        CutsceneManager cutsceneManager;
        private SkillFromUiIconClickController skillFromUiIconClickController;
        private void Start()
        {
            combatantsManager = FindObjectOfType<CombatantsManager>();
            cutsceneManager = FindObjectOfType<CutsceneManager>();
            skillFromUiIconClickController = GetComponent<SkillFromUiIconClickController>();
        }

        // Update is called once per frame
        void Update()
        {
            if (cutsceneManager.IsCutsceneActive)
            {
                return;
            }
            if (EventSystem.current.IsPointerOverGameObject())
            {
                // We are over UI, do not do commands.
                return;
            }
            if (UnityEngine.Input.GetMouseButtonUp(1))
            {
                Vector3 mousePos = UnityEngine.Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
                Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

                RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

                Hero speakingHero = null;
                VoiceOrderType? orderType = null;
                if (skillFromUiIconClickController.IsUsingSkill)
                {
                    speakingHero = skillFromUiIconClickController.CastingHero;
                    TryUseSkillFromUi(hit, out orderType);
                    skillFromUiIconClickController.ClearUsedSkill();
                }
                else
                {
                    ResolveRaycast(hit, out speakingHero, out orderType);
                }

                var speakingCharacterVoice = speakingHero != null ? speakingHero.GetComponentInChildren<CharacterVoiceController>() : null;
                if (orderType != null && speakingCharacterVoice != null)
                {
                    speakingCharacterVoice.OnOrderGiven(orderType.Value);
                }
            }
        }

        private void TryUseSkillFromUi(RaycastHit2D hit, out VoiceOrderType? orderType)
        {
            orderType = null;
            var hitCombatant = hit.collider != null ? hit.collider.GetComponent<CombatantBase>() : null;
            if (hitCombatant == null || (hitCombatant is Hero) != skillFromUiIconClickController.IsFriendlySkill)
            {
                // Invalid target - either not a hostile or friendly when it is supposed to be the other way around or not a combatant.
                return;
            }

            if (skillFromUiIconClickController.TargetedSkill.UseSkillOn(hitCombatant))
            {
                orderType = skillFromUiIconClickController.IsFriendlySkill
                    ? VoiceOrderType.FriendlySkill
                    : VoiceOrderType.EnemySkill;
            }
        }

        private void ResolveRaycast(RaycastHit2D hit, out Hero speakingCharacter, out VoiceOrderType? orderType)
        {
            var usingSkill = UnityEngine.Input.GetKey(KeyCode.LeftControl) || UnityEngine.Input.GetKey(KeyCode.RightControl);
            var selectedCharacters = combatantsManager.GetPlayerCharacters(onlySelected: true).ToList();
            speakingCharacter = selectedCharacters.FirstOrDefault();
            var hitEnemy = hit.collider != null ? hit.collider.GetComponent<Monster>() : null;
            var hitFriend = hit.collider != null ? hit.collider.GetComponent<Hero>() : null;
            var hitInteractableObject = hit.collider != null ? hit.collider.GetComponent<InteractableObject>() : null;
            // Hacky - basically we need to make sure that if one character opened doors, others won't try to go through them.
            // So if one suceeds in using an interactive item straight away, others won't try it.
            bool didUserInteractableObject = false;
            orderType = null;
            foreach (var character in selectedCharacters)
            {
                if (hitInteractableObject)
                {
                    orderType = VoiceOrderType.Move;
                    if (hitInteractableObject.IsHeroCloseToInteract(character))
                    {
                        didUserInteractableObject = hitInteractableObject.TryInteract(character);
                    }
                    else if (!didUserInteractableObject)
                    {
                        // We cache a temporary reference to the object, as we will be clearing
                        // it before the completion completes.
                        var interactableObject = hitInteractableObject;
                        character.GetComponent<MovementController>().MoveToPosition(hit.collider.transform.position,
                            onMoveToSuccessful: moveSuccess =>
                            {
                                if (moveSuccess)
                                {
                                    interactableObject.TryInteract(character);
                                }
                            });
                    }

                    // Else nothing - the interactive item was used by someone else, this character has nothing to do.
                }
                else if (hitEnemy)
                {
                    if (usingSkill)
                    {
                        orderType = VoiceOrderType.EnemySkill;
                        character.SkillAttackUsed(hitEnemy);
                    }
                    else
                    {
                        orderType = VoiceOrderType.Attack;
                        character.AttackUsed(hitEnemy);
                    }
                }
                else if (hitFriend == character)
                {
                    if (usingSkill)
                    {
                        if (character == speakingCharacter)
                        {
                            orderType = VoiceOrderType.SelfSkill;
                        }

                        character.SelfSkillUsed();
                    }
                    else
                    {
                        character.SelfClicked();
                    }
                }
                else if (hitFriend)
                {
                    if (usingSkill)
                    {
                        if (character == speakingCharacter)
                        {
                            orderType = VoiceOrderType.FriendlySkill;
                        }

                        character.FriendlySkillUsed(hitFriend);
                    }
                    else
                    {
                        character.FriendlyClicked(hitFriend);
                    }
                }
                else
                {
                    orderType = VoiceOrderType.Move;
                    var clickTarget = UnityEngine.Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
                    if (usingSkill)
                    {
                        character.LocationSkillClick(clickTarget);
                    }
                    else
                    {
                        character.LocationClick(clickTarget);
                    }
                }
            }
        }
    }
}
