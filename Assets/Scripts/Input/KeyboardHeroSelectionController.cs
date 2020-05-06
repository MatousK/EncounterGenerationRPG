using Assets.Scripts.Combat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Input
{
    class KeyboardHeroSelectionController: MonoBehaviour
    {
        private void Update()
        {
            if (UnityEngine.Input.GetKeyUp(KeyCode.Alpha1))
            {
                SelectHero(0);
            }
            if (UnityEngine.Input.GetKeyUp(KeyCode.Alpha2))
            {
                SelectHero(1);
            }
            if (UnityEngine.Input.GetKeyUp(KeyCode.Alpha3))
            {
                SelectHero(2);
            }
        }

        private void SelectHero(int index)
        {
            var combatantsManager = FindObjectOfType<CombatantsManager>();
            UnityEngine.Debug.Assert(combatantsManager != null);
            if (combatantsManager == null || index >= combatantsManager.PlayerCharacters.Count)
            {
                UnityEngine.Debug.Assert(index < combatantsManager.PlayerCharacters.Count);
                return;
            }
            var heroToSelect = combatantsManager.PlayerCharacters[index];
            if (heroToSelect.IsDown)
            {
                // Do not select dead heroes.
                return;
            }
            var holdingShift = UnityEngine.Input.GetKey(KeyCode.LeftShift) || UnityEngine.Input.GetKey(KeyCode.RightShift);
            if (!holdingShift)
            {
                foreach (var combatant in combatantsManager.PlayerCharacters)
                {
                    combatant.GetComponent<SelectableObject>().IsSelected = false;
                }
            }
            heroToSelect.GetComponent<SelectableObject>().IsSelected = true;
        }
    }
}
