using Assets.Scripts.Combat;
using UnityEngine;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// Base class for progress bars showing something related to a combatant.
    /// Every frame updates these indicators to match the combatant's state.
    /// </summary>
    public abstract class UiProgressBarBase: MonoBehaviour
    {
        /// <summary>
        /// The class representing the combatant. Expected to be in this object or parent.
        /// </summary>
        protected CombatantBase RepresentedCombatant;
        protected virtual void Awake()
        {
            RepresentedCombatant = GetComponentInParent<CombatantBase>();
        }
        /// <summary>
        /// Called before first frame. Updates indicators to match the <see cref="RepresentedCombatant"/>.
        /// </summary>
        protected virtual void Start()
        {
            UpdateIndicators();
        }

        /// <summary>
        /// Called every frame. Updates indicators to match the <see cref="RepresentedCombatant"/>.
        /// </summary>
        protected virtual void Update()
        {
            UpdateIndicators();
            // Ensure that if the parent is flipped, this is flipped too, so in the end, nothing is flipped.
            transform.localScale = new Vector3(transform.parent.localScale.x, 1, 1);
        }
        /// <summary>
        /// Child classes should override this to change the value they are showing based on the values from <see cref="RepresentedCombatant"/>.
        /// </summary>
        protected abstract void UpdateIndicators();
    }
}