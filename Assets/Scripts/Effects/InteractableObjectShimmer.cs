using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Effects
{
    /// <summary>
    /// Shimmering objects displayed over usable objects.
    /// Is displayed only if the object has not been used and is usable at the moment.
    /// For example, doors and chests are not usable during combat.
    /// Object with this component should also have an animation component that should do the shimmer effect.
    /// </summary>
    public class InteractableObjectShimmer: MonoBehaviour
    {
        /// <summary>
        /// Should be set to true once the item is used, because
        /// it probably wont be used again.
        /// </summary>
        [HideInInspector]
        public bool ObjectAlreadyUsed;
        /// <summary>
        /// Should be true if the item cannot be used right now,
        /// e.g. it is not usable in combat and we are in combat.
        /// </summary>
        [HideInInspector]
        public bool IsNotUsableRightNow;

        private SpriteRenderer effectRenderer;

        private void Start()
        {
            effectRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            bool shouldShowEffect = !ObjectAlreadyUsed && !IsNotUsableRightNow;
            if (effectRenderer.enabled != shouldShowEffect)
            {
                effectRenderer.enabled = shouldShowEffect;
                if (shouldShowEffect)
                {
                    GetComponent<Animation>().Play();
                }
            }
        }
    }
}
