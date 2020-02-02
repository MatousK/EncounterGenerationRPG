using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RightClickController : MonoBehaviour
{
    CombatantsManager combatantsManager;
    CutsceneManager cutsceneManager;
    private void Awake()
    {
        combatantsManager = FindObjectOfType<CombatantsManager>();
        cutsceneManager = FindObjectOfType<CutsceneManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (cutsceneManager.IsCutsceneActive)
        {
            return;
        }
        if (Input.GetMouseButtonUp(1))
        {
            var usingSkill = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

            var hitEnemy = hit.collider != null ? hit.collider.GetComponent<Monster>() : null;
            var hitFriend = hit.collider != null ? hit.collider.GetComponent<Hero>() : null;
            var hitInteractableObject = hit.collider != null ? hit.collider.GetComponent<InteractableObject>() : null;
            // Hacky - basically we need to make sure that if one character opened doors, others won't try to go through them.
            // So if one suceeds in using an interactive item straight away, others won't try it.
            bool didUserInteractableObject = false;
            foreach (var character in combatantsManager.GetPlayerCharacters(onlySelected: true))
            {
                if (hitInteractableObject)
                {
                    if (hitInteractableObject.IsHeroCloseToInteract(character))
                    {
                        didUserInteractableObject = hitInteractableObject.TryInteract(character);
                    }
                    else if (!didUserInteractableObject)
                    {
                        // We cache a temporary reference to the object, as we will be clearing
                        // it before the completion completes.
                        var interactableObject = hitInteractableObject;
                        character.GetComponent<MovementController>().MoveToPosition(hit.collider.transform.position, onMoveToSuccessful: moveSuccess =>
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
                        character.SkillAttackUsed(hitEnemy);
                    }
                    else
                    {
                        character.AttackUsed(hitEnemy);
                    }
                }
                else if (hitFriend == character)
                {
                    if (usingSkill)
                    {
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
                        character.FriendlySkillUsed(hitFriend);
                    }
                    else
                    {
                        character.FriendlyClicked(hitFriend);
                    }
                }
                else
                {
                    var clickTarget = Camera.main.ScreenToWorldPoint(Input.mousePosition);
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
