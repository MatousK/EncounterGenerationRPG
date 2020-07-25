using System.Collections.Generic;
using Assets.Scripts.Combat;
using Assets.Scripts.UI.Skills;
using UnityEngine;

namespace Assets.Scripts.UI.CharacterPortrait
{
    /// <summary>
    /// This component assigns to character containers and skill containers the appropriate hero they represent.
    /// </summary>
    public class CharacterPortraitsManager: MonoBehaviour
    {
        /// <summary>
        /// Ordered list of available portrait widgets.
        /// Must be in the same order as <see cref="AvailableSkillsContainers"/>.
        /// </summary>
        public List<CharacterPortrait> AvailablePortraitWidgets;
        /// <summary>
        /// Ordered list of available containers which display the skills of some hero..
        /// Must be in the same order as <see cref="AvailablePortraitWidgets"/>.
        /// </summary>
        public List<HeroSkillsContainer> AvailableSkillsContainers;
        /// <summary>
        /// The class which knows about all combatants in the game.
        /// </summary>
        private CombatantsManager combatantsManager;
        /// <summary>
        /// Update the heroes the portraits are showing.
        /// </summary>
        void Update()
        {
            if (combatantsManager == null)
            {
                combatantsManager = FindObjectOfType<CombatantsManager>();
                if (combatantsManager == null)
                {
                    // Game is probably not loaded yet;
                    return;
                }
            }
            UpdatePortraits();
        }
        /// <summary>
        /// For each portrait and skill container, update the combatant class it is representing.
        /// </summary>
        void UpdatePortraits()
        {
            UnityEngine.Debug.Assert(AvailableSkillsContainers.Count == AvailablePortraitWidgets.Count);
            var currentPartyMembers = combatantsManager.PlayerCharacters;
            for (int i = 0; i < AvailablePortraitWidgets.Count; ++i)
            {
                if (i < currentPartyMembers.Count)
                {
                    AvailablePortraitWidgets[i].gameObject.SetActive(true);
                    AvailablePortraitWidgets[i].RepresentedHero = currentPartyMembers[i];

                    AvailableSkillsContainers[i].gameObject.SetActive(true);
                    AvailableSkillsContainers[i].RepresentedHero = currentPartyMembers[i];
                }
                else
                {
                    AvailablePortraitWidgets[i].gameObject.SetActive(false);
                    AvailableSkillsContainers[i].gameObject.SetActive(false);
                }
            }
        }
    }
}