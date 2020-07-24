using System;
using UnityEngine;
using Assets.Scripts.Combat.Skills.Monster.Lurker;

namespace Assets.Scripts.Effects
{
    /// <summary>
    /// This component is attached to the effect that is played when the <see cref="FireTeleport"/> is activated.
    /// When the <see cref="StartFire"/> method is called, this component will play the animation of the fire engulfing the character <see cref="FireAnimationRepetitions"/> times,
    /// raising events on important parts of the effect.
    /// </summary>
    public class FireTeleportEffect: MonoBehaviour
    {
        /// <summary>
        /// How many times should the animation be repeated per one call to <see cref="StartFire"/>
        /// </summary>
        public int FireAnimationRepetitions = 2;
        /// <summary>
        /// This event is raised when the fire reaches its maximum height.
        /// </summary>
        public event EventHandler OnFireMaxSize;
        /// <summary>
        /// This is called when each repetition of the fire animation ends.
        /// So this will be called multiple times per one call to <see cref="StartFire"/> method.
        /// </summary>
        public event EventHandler OnFireAnimationEnded;
        /// <summary>
        /// Counts the current repetition of the fire animation. Cleared on every call to <see cref="StartFire"/>
        /// </summary>
        private int currentFireAnimationRepetions;
        /// <summary>
        /// Starts the effect this component controls, starting the animation and clearing <see cref="currentFireAnimationRepetions"/>
        /// </summary>
        public void StartFire()
        {
            currentFireAnimationRepetions = 0;
            GetComponent<Animator>().SetBool("FireTeleportationActive", true);
        }
        /// <summary>
        /// Called when the fire reaches its maximum size during the animation.
        /// </summary>
        public void FireMaxSize()
        {
            OnFireMaxSize?.Invoke(this, new EventArgs());
        }
        /// <summary>
        /// Called when one repetition of the fire animation ends.
        /// If we have already done <see cref="FireAnimationRepetitions"/> repetitions, stop the animation.
        /// </summary>
        public void FireEnded()
        {
            OnFireAnimationEnded?.Invoke(this, new EventArgs());
            if (++currentFireAnimationRepetions >= FireAnimationRepetitions)
            {
                GetComponent<Animator>().SetBool("FireTeleportationActive", false);
            }
        }
    }
}