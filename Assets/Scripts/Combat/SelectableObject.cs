using Assets.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts.Combat
{
    public class SelectableObject : MonoBehaviour
    {
        public bool IsSelected;
        /// <summary>
        /// If false then, for whatever reason, selection is right now impossible.
        /// </summary>
        public bool IsSelectionEnabled = true;
        Circle selectionIndicator;

        // Start is called before the first frame update
        void Awake()
        {
            selectionIndicator = GetComponent<Circle>();
            selectionIndicator.IsVisible = IsSelected;
            var combatant = GetComponent<CombatantBase>();
            if (combatant != null)
            {
                combatant.CombatantDied += Combatant_CombatantDied;
            }
        }

        // Update is called once per frame
        void Update()
        {
            selectionIndicator.IsVisible = IsSelected;
        }

        private void Combatant_CombatantDied(object sender, System.EventArgs e)
        {
            IsSelected = false;
        }
    }
}
