using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Animations;
using UnityEngine;

namespace Assets.Scripts.Sound
{
    /// <summary>
    /// Component which plays the footsteps of the character it is attached to.
    /// </summary>
    public class FootstepsController : MonoBehaviour
    {
        /// <summary>
        /// Footsteps sounds for the current character. Will be played in this order.
        /// </summary>
        public List<AudioClip> FootstepsSounds = new List<AudioClip>();
        /// <summary>
        /// The index of the footstep that should play next.
        /// </summary>
        private int currentFootstepIndex = 0;
        /// <summary>
        /// The audio source that plays the footsteps.
        /// </summary>
        private AudioSource audioSource;
        /// <summary>
        /// Listens to the events from the animation listener and plays the footsteps sounds when the appropriate event is called.
        /// </summary>
        private AnimationEventsListener animationEventsListener;
        /// <summary>
        /// Executed before the first update.
        /// Finds dependencies in and binds to events.
        /// </summary>
        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            animationEventsListener = GetComponentInParent<AnimationEventsListener>();
            animationEventsListener.PlaySoundEffectRequested += AnimationEventsListener_PlaySoundEffectRequested;
        }
        /// <summary>
        /// When destroyed, unbinds from events.
        /// </summary>
        private void OnDestroy()
        {
            animationEventsListener.PlaySoundEffectRequested -= AnimationEventsListener_PlaySoundEffectRequested;
        }
        /// <summary>
        /// Animation reached the point when it wants us to play the footstep, play it
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Type of the sound effect that should play. Everything except <see cref="SoundEffectType.Footstep"/> is ignored.</param>
        private void AnimationEventsListener_PlaySoundEffectRequested(object sender, SoundEffectType e)
        {
            if (e == SoundEffectType.Footstep)
            {
                OnFootstep();
            }
        }
        /// <summary>
        /// Called when a footstep effect should be played. Plays the next footstep in sequence.
        /// </summary>
        public void OnFootstep()
        {
            if (!FootstepsSounds.Any())
            {
                return;
            }
            var currentClip = FootstepsSounds[currentFootstepIndex];
            audioSource.PlayOneShot(currentClip);
            if (++currentFootstepIndex >= FootstepsSounds.Count)
            {
                currentFootstepIndex = 0;
            }
        }
    }
}
