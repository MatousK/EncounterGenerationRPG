using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Tutorial
{
    /// <summary>
    /// This is a single tutorial step. 
    /// When the player does the action required by the tutorial, <see cref="IsTutorialStepOver"/> should start returning true and we should move to the next step.
    /// </summary>
    public abstract class TutorialStep: MonoBehaviour
    {
        /// <summary>
        /// Returns true when this tutorial step should end and we should move to the next one.
        /// </summary>
        /// <returns>True if this tutorial step is over.</returns>
        public abstract bool IsTutorialStepOver();
    }
}
