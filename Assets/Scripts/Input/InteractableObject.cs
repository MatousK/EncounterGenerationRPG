using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
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
    /// How close must the character be to trigger the interaction.
    /// </summary>
    public float MaxDistanceToInteract = 1;
    /// <summary>
    /// Called when a character gets close enough to trigger the interaction. The argument is the character who triggered the interaction.
    /// </summary>
    public event EventHandler<Hero> OnInteractionTriggered;

    private CombatantsManager combatantsManager;
    public void Start()
    {
        combatantsManager = FindObjectOfType<CombatantsManager>();
    }
    /// <summary>
    /// Starts the interaction with this object by the hero if possible.
    /// </summary>
    /// <param name="interactingHero">The hero who wishes to interact with this object.</param>
    /// <returns>True if the interaction was successful, otherwise false.</returns>
    public bool TryInteract(Hero interactingHero)
    { 
        if ((!AllowedInCombat && combatantsManager.IsCombatActive) || !IsHeroCloseToInteract(interactingHero))
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

    public bool IsHeroCloseToInteract(Hero hero)
    {
        Vector2 heroPosition2D = hero.transform.position;
        Vector2 selfPosition2D = transform.position;
        return Vector2.Distance(heroPosition2D, selfPosition2D) <= MaxDistanceToInteract;
    }
}
    