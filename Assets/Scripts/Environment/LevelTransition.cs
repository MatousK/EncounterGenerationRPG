using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Combat;
using Assets.Scripts.GameFlow;
using Assets.Scripts.Input;
using UnityEngine;

namespace Assets.Scripts.Environment
{
    /// <summary>
    /// The component which can trigger the transition to the next level.
    /// Required that the object has the <see cref="InteractableObject"/> to detect that the character interacted with this object.
    /// </summary>
    public class LevelTransition: MonoBehaviour
    {
        /// <summary>
        /// If true, the player already started the transition, so the next click on the doors should not do anything.
        /// </summary>
        private bool didTransition;
        private void Start()
        {
            GetComponent<InteractableObject>().OnInteractionTriggered += LevelTransitionActivated;
        }
        /// <summary>
        /// Called when the heroes start the level transition.
        /// Only once, further calls do nothing.
        /// Triggers the next level transition on the <see cref="LevelLoader"/>
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="hero">The hero who triggered the event.</param>
        private void LevelTransitionActivated(object sender, Hero hero)
        {
            if (!didTransition)
            {
                FindObjectOfType<LevelLoader>().LoadNextLevel();
                didTransition = true;
            }
        }
    }
}
