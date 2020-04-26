using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Tutorial
{
    public abstract class TutorialStep: MonoBehaviour
    {
        public abstract bool IsTutorialStepOver();
    }
}
