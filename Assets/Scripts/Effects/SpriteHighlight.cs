using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Effects
{
    /// <summary>
    /// A component that will every frame update the <see cref="SpriteMask"/> component on this object to the sprite of some renderer.
    /// Used to create a mask that use as their source an animation object, e.g. a character. Does the highlight effect on mouse over.
    /// </summary>
    class SpriteHighlight: MonoBehaviour
    {
        /// <summary>
        /// The renderer whose current mask should be used as a mask for the <see cref="SpriteMask"/> component on the game object this component is attached to.
        /// </summary>
        public SpriteRenderer MaskTemplate;

        private void Update()
        {
            GetComponent<SpriteMask>().sprite = MaskTemplate.sprite;
        }
    }
}
