using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Effects
{
    /// <summary>
    /// A component on objects which should be highlighted when the mouse is over them.
    /// </summary>
    class HighlightableObject: MonoBehaviour
    {
        /// <summary>
        /// The object that, when active, highlights the character.
        /// </summary>
        public GameObject HighlightEffect;
        /// <summary>
        /// If true, the current character should be highlighted.
        /// </summary>
        public bool IsHighlighted;
        void Start()
        {
            HighlightEffect.SetActive(IsHighlighted);
        }

        void Update()
        {
            IsHighlighted = false;
        }

        void LateUpdate()
        {
            // MouseOverHighlightController will set IsHighlighted to true between update and LateUpdate.
            HighlightEffect.SetActive(IsHighlighted);
        }
    }
}
