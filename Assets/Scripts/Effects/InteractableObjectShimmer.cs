using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Effects
{
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
