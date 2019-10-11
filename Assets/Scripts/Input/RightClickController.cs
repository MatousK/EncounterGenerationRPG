﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RightClickController : MonoBehaviour
{
    CombatantsManager combatantsManager;
    private void Start()
    {
        combatantsManager = FindObjectOfType<CombatantsManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(1))
        {
            var usingSkill = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

            var hitEnemy = hit.collider?.GetComponent<Enemy>();
            var hitFriend = hit.collider?.GetComponent<Character>();
            foreach (var character in combatantsManager.GetPlayerCharacters(onlySelected: true))
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
