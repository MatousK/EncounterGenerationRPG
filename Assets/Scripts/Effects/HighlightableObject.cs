using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Effects
{
    class HighlightableObject: MonoBehaviour
    {
        public GameObject HighlightEffect;
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
