using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Combat;
using Assets.Scripts.Cutscenes;
using Assets.Scripts.Environment;
using Assets.Scripts.Movement;
using Assets.Scripts.Sound.CharacterSounds;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Input
{
    /// <summary>
    /// Manages all the things that can happen when the user rights clicks in the game. Does not handle UI.
    /// </summary>
    public class RightClickController : MonoBehaviour
    {
        /// <summary>
        /// This component knows about all combatants in the game.
        /// </summary>
        CombatantsManager combatantsManager;
        /// <summary>
        /// This component knows about cutscenes, which is necessary as cutscenes block input.
        /// </summary>
        CutsceneManager cutsceneManager;
        /// <summary>
        /// This component tells us whether user is currently using a skill by clicking on a skill icon in the UI.
        /// </summary>
        private SkillFromUiIconClickController skillFromUiIconClickController;
        /// <summary>
        /// Called before the first update. Finds references to all dependencies.
        /// </summary>
        private void Start()
        {
            combatantsManager = FindObjectOfType<CombatantsManager>();
            cutsceneManager = FindObjectOfType<CutsceneManager>();
            skillFromUiIconClickController = GetComponent<SkillFromUiIconClickController>();
        }

        /// <summary>
        /// Update is called once per frame.
        /// Handles the right click when it happens.
        /// </summary>
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
                // Right click happen, see what is under the cursor.
                Vector3 mousePos = UnityEngine.Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
                Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

                RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

                Hero speakingHero;
                VoiceOrderType? orderType;
                if (skillFromUiIconClickController.IsUsingSkill)
                {
                    // If we are currently using a skill from the UI, we try to use the skill on the thing we hit.
                    speakingHero = skillFromUiIconClickController.CastingHero;
                    TryUseSkillFromUi(hit, out orderType);
                    skillFromUiIconClickController.ClearUsedSkill();
                }
                else
                {
                    // No skill being used, just try to handle the right click on the object under the cursor.
                    ResolveRaycast(hit, out speakingHero, out orderType);
                }
                // If the right click did something, it filled the speakingHero and orderType variables with the hero who did the command
                // and the type of command that happened. Try to play the sound effect related to that order.
                var speakingCharacterVoice = speakingHero != null ? speakingHero.GetComponentInChildren<CharacterVoiceController>() : null;
                if (orderType != null && speakingCharacterVoice != null)
                {
                    speakingCharacterVoice.OnOrderGiven(orderType.Value);
                }
            }
        }
        /// <summary>
        /// Check if the object under the cursor is a valid target for this skill.
        /// </summary>
        /// <param name="hit">The result of the raycast under the cursor.</param>
        /// <param name="orderType">Output parameter, should be set to the type of order we executed. </param>
        private void TryUseSkillFromUi(RaycastHit2D hit, out VoiceOrderType? orderType)
        {
            orderType = null;
            // Try retrieve the combatant we hit if we hit one.
            var hitCombatant = hit.collider != null ? hit.collider.GetComponent<CombatantBase>() : null;
            if (hitCombatant == null || (hitCombatant is Hero) != skillFromUiIconClickController.IsFriendlySkill || hitCombatant == skillFromUiIconClickController.CastingHero)
            {
                // Invalid target - either not a hostile or friendly when it is supposed to be the other way around or not a combatant. Or self, we can't do that either.
                return;
            }
            // We hit a valid target, use the skill.
            if (skillFromUiIconClickController.IsFriendlySkill)
            {
                orderType = VoiceOrderType.FriendlySkill;
                skillFromUiIconClickController.CastingHero.FriendlySkillUsed(hitCombatant as Hero);
            }
            else
            {
                orderType = VoiceOrderType.EnemySkill;
                skillFromUiIconClickController.CastingHero.SkillAttackUsed(hitCombatant as Monster);
            }
        }
        /// <summary>
        /// Figure out what exactly is the thing we hit with the raycast under the cursor and wheter we can do something about it.
        /// </summary>
        /// <param name="hit"></param>
        /// <param name="speakingCharacter"></param>
        /// <param name="orderType"></param>
        private void ResolveRaycast(RaycastHit2D hit, out Hero speakingCharacter, out VoiceOrderType? orderType)
        {
            // Holding CTRL means we want to use a skill.
            var usingSkill = UnityEngine.Input.GetKey(KeyCode.LeftControl) || UnityEngine.Input.GetKey(KeyCode.RightControl);
            // Any command is given to the selected characters.
            var selectedCharacters = combatantsManager.GetPlayerCharacters(onlySelected: true).ToList();
            speakingCharacter = selectedCharacters.FirstOrDefault();
            var hitEnemy = hit.collider != null ? hit.collider.GetComponent<Monster>() : null;
            var hitFriend = hit.collider != null ? hit.collider.GetComponent<Hero>() : null;
            var hitInteractableObject = hit.collider != null ? hit.collider.GetComponent<InteractableObject>() : null;
            // Hacky - basically we need to make sure that if one character opened doors, others won't try to go through them.
            // So if one suceeds in using an interactive item straight away, others won't try it.
            bool didUserInteractableObject = false;
            orderType = null;
            // Give the order to every selected character.
            foreach (var character in selectedCharacters)
            {
                // Huge if resolving how to manage the hit.
                // TODO: Refactor so we have some registered right click handlers for different classes that we can hit, ifs are unwieldy.
                if (hitInteractableObject)
                {
                    // So, what's happening here - Without this special handling of treasure chests,
                    // if the player had multiple characters selected and right clicked on the chest, all of them would go there.
                    // When they reach it the first would open it, second would apply the powerup.
                    // And the player would not even see what it was. This is a hacky workaround to ensure that pickup and opening of chest must be separate clicks.
                    // Because isOpened won't be true until some next frame when the characters reach the chest.
                    // TODO: Actually, it will if one character is standing close enough now that I read this. We should fix that.
                    var treasureChest = hitInteractableObject.GetComponent<TreasureChest>();
                    if (treasureChest != null && treasureChest.IsOpened)
                    {
                        treasureChest.AllowPowerupPickup = true;
                    }
                    orderType = VoiceOrderType.Move;
                    if (hitInteractableObject.IsHeroCloseToInteract(character))
                    {
                        didUserInteractableObject = hitInteractableObject.TryInteract(character);
                    }
                    else if (!didUserInteractableObject)
                    {
                        // We cache a temporary reference to the object, as we will be clearing
                        // it before the completion completes.
                        // Move to the object and when we reach it, use it.
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
                // The rest of these are easy - give the appropriate order to the hero based on what he hit and whether he is using a skill.
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
                    // And if we hit nothing interesting, it is a move command to the target position.
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
