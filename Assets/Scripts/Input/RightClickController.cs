using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RightClickController : MonoBehaviour
{
    SelectionController selectionController;
    private void Start()
    {
        selectionController = GetComponent<SelectionController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(1))
        {
            var usingSkill = Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl);
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

            var hitEnemy = hit.collider?.GetComponent<Enemy>();
            var hitFriend = hit.collider?.GetComponent<Character>();
            var selectedCharacters = selectionController.GetSelectedCharacters();
            foreach (var character in selectedCharacters)
            {
                if (hitEnemy)
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
