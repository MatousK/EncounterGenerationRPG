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
            foreach (var character in combatantsManager.GetPlayerCharacters(onlySelected: true))
            {
                if (hitInteractableObject)
                {
                    if (hitInteractableObject.IsHeroCloseToInteract(character))
                    {
                        hitInteractableObject.TryInteract(character);
                    }
                    else
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
                    // This is makes sure that only one character tries to use an object at once, to prevent race conditions. The other will just walk to the position.
                    hitInteractableObject = null;
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
