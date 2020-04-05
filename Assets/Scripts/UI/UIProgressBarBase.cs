using Assets.Scripts.Combat;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public abstract class UiProgressBarBase: MonoBehaviour
    {
        protected CombatantBase RepresentedCombatant;
        protected virtual void Awake()
        {
            RepresentedCombatant = GetComponentInParent<CombatantBase>();
        }
        protected virtual void Start()
        {
            UpdateIndicators();
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            UpdateIndicators();
            // Ensure that if the parent is flipped, this is flipped too, so in the end, nothing is flipped.
            transform.localScale = new Vector3(transform.parent.localScale.x, 1, 1);
        }
        protected abstract void UpdateIndicators();
    }
}