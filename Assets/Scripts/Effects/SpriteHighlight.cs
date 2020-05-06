using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Effects
{
    class SpriteHighlight: MonoBehaviour
    {
        public SpriteRenderer MaskTemplate;

        private void Update()
        {
            GetComponent<SpriteMask>().sprite = MaskTemplate.sprite;
        }
    }
}
