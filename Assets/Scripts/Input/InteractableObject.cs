using System;
using Assets.Scripts.Combat;
using Assets.Scripts.Effects;
using UnityEngine;

namespace Assets.Scripts.Input
{
    /// <summary>
    /// Object on which on which it is possible to right click. The character will go to this object on right click and once close enough he will trigger the interaction
    /// </summary>
    public class InteractableObject: MonoBehaviour
    {
        /// <summary>
        /// If true, it is possible to use this item in combat.
        /// </summary>
        public bool AllowedInCombat = false;
        /// <summary>
        /// If true, no interaction can be triggered because the tutorial says so.
        /// </summary>
        public bool IsInteractionDisabledByTutorial;
        /// <summary>
        /// How close must the character be to trigger the interaction.
        /// </summary>
        public float MaxDistanceToInteract = 1;
        /// <summary>
        /// Called when a character gets close enough to trigger the interaction. The argument is the character who triggered the interaction.
        /// </summary>
        public event EventHandler<Hero> OnInteractionTriggered;
        /// <summary>
        /// The effect that indicates that the object is usable.
        /// This class controls whether it should be active or not.
        /// </summary>
        private InteractableObjectShimmer shimmerEffect;
        /// <summary>
        /// Contains reference to all combatants in the game.
        /// Used to detect whether we are in combat.
        /// </summary>
        private CombatantsManager combatantsManager;
        /// <summary>
        /// Called before the first frame update. Initializes references to dependencies.
        /// </summary>
        private void Start()
        {
            combatantsManager = FindObjectOfType<CombatantsManager>();
            shimmerEffect = GetComponentInChildren<InteractableObjectShimmer>();
        }
        /// <summary>
        /// Called every frame. Updates whether the shimmer animation should be playing right now.
        /// </summary>
        private void Update()
        {
            if (shimmerEffect == null)
            {
                return;
            }

            shimmerEffect.IsNotUsableRightNow = !AllowedInCombat && combatantsManager.IsCombatActive;
        }

        /// <summary>
        /// Starts the interaction with this object by the hero if possible.
        /// Only does something if the hero is close enough to the object.
        /// </summary>
        /// <param name="interactingHero">The hero who wishes to interact with this object.</param>
        /// <returns>True if the interaction was successful, otherwise false.</returns>
        public bool TryInteract(Hero interactingHero)
        { 
            if ((!AllowedInCombat && combatantsManager.IsCombatActive) || !IsHeroCloseToInteract(interactingHero) || IsInteractionDisabledByTutorial)
            {
                return false;
            }

            if (OnInteractionTriggered == null)
            {
                throw new InvalidOperationException("Trying to interact with an object that does not have any interaction handler registered.");
            }
            OnInteractionTriggered(this, interactingHero);
            return true;
        }
        /// <summary>
        /// Return true if the hero is close enough to the object to trigger interaction.
        /// If he is not, someone should order the hero to move here.
        /// </summary>
        /// <param name="hero">The hero who wants to interact.</param>
        /// <returns>True if the hero is close enough to the object, otherwise false.</returns>
        public bool IsHeroCloseToInteract(Hero hero)
        {
            if (GetComponent<Collider2D>() != null)
            {
                return GetComponent<Collider2D>().Distance(hero.GetComponent<Collider2D>()).distance <= MaxDistanceToInteract;
            }
            Vector2 heroPosition2D = hero.transform.position;
            Vector2 selfPosition2D = transform.position;
            return Vector2.Distance(heroPosition2D, selfPosition2D) <= MaxDistanceToInteract;
        }
    }
}
    