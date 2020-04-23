using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Combat;
using Assets.Scripts.Combat.Skills;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Input
{
    class SkillFromUiIconClickController: MonoBehaviour
    {
        public TargetedSkill TargetedSkill;
        public bool IsFriendlySkill;
        public bool IsUsingSkill => TargetedSkill != null;
        private RightClickController rightClickController;
        private LeftClickController leftClickController;
        private void Start()
        {
            rightClickController = GetComponent<RightClickController>();
            leftClickController = GetComponent<LeftClickController>();
        }

        private void Update()
        {
            rightClickController.enabled = !IsUsingSkill;
            leftClickController.enabled = !IsUsingSkill;
        }

        public void TryUseSkillOnCombatantUnderCursor()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            // HACK: Logically, this should be for when the button is released.
            // However, selected skill is tied to the button losing focus.
            // And the focus is lost on button down, not on button up.
            // So we hae to check for button down.
            if (UnityEngine.Input.GetMouseButton(1))
            {
                var targetedCombatant = GetCombatantUnderCursor();
                // There is a target and it either is a hero and we are using a friendly skill, or it s not a hero and we are not using a friendly skill.
                if (targetedCombatant != null && targetedCombatant is Hero == IsFriendlySkill)
                {
                    TargetedSkill.UseSkillOn(targetedCombatant);
                }
            }
        }

        private CombatantBase GetCombatantUnderCursor()
        {
            Vector3 mousePos = UnityEngine.Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            if (hit.collider != null)
            {
                return hit.collider.GetComponent<CombatantBase>();
            }
            else
            {
                return null;
            }
        }
    }
}
