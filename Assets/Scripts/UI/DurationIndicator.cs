using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class DurationIndicator: MonoBehaviour
    {
        public float TotalDuration;
        public float RemainingDuration;
        private void Update()
        {
            var image = GetComponent<Image>();
            if (TotalDuration != 0)
            {
                image.fillAmount = RemainingDuration / TotalDuration;
            }
        }
    }
}
