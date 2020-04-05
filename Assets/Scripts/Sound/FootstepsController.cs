using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Animations;
using UnityEngine;

namespace Assets.Scripts.Sound
{
    public class FootstepsController : MonoBehaviour
    {
        public List<AudioClip> FootstepsSounds = new List<AudioClip>();
        private int currentFootstepIndex = 0;
        private AudioSource audioSource;
        private AnimationEventsListener animationEventsListener;
        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            animationEventsListener = GetComponentInParent<AnimationEventsListener>();
            animationEventsListener.SoundCompleted += AnimationEventsListener_SoundCompleted;
        }

        private void AnimationEventsListener_SoundCompleted(object sender, SoundEffectType e)
        {
            if (e == SoundEffectType.Footstep)
            {
                OnFootstep();
            }
        }

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
