using Assets.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts.Combat
{
    /// <summary>
    /// Game object with this component supports being selected by the player.
    /// </summary>
    public class SelectableObject : MonoBehaviour
    {
        /// <summary>
        /// If true, this component is currently selected.
        /// </summary>
        public bool IsSelected;
        /// <summary>
        /// If false then, for whatever reason, selection is right now impossible.
        /// </summary>
        public bool IsSelectionEnabled = true;
        /// <summary>
        /// Circle that should be visible when the object is selected.
        /// </summary>
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
        /// <summary>
        /// Unselects the hero when the represented combatant dies.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void Combatant_CombatantDied(object sender, System.EventArgs e)
        {
            IsSelected = false;
        }
    }
}
