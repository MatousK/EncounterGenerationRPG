using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;

namespace Assets.Scripts.Sound.Music
{
    public class MusicTransitionManger: MonoBehaviour
    {
        public float TransitionDuration = 1f;
        public AudioMixerGroup MixerGroup;
        private readonly List<AudioSource> musicAudioSources = new List<AudioSource>();
        private int activeAudioSourceIndex = -1;
        /// <summary>
        ///  How long will we wait before changing volume in a transition, on seconds
        /// </summary>
        private const float TransitionStepLength = 0.1f;
        IEnumerator activeTransition;
        void Awake()
        {
            var musicControllers = FindObjectsOfType<MusicTransitionManger>();
            if (musicControllers.Length > 1)
            {
                Destroy(gameObject);
            }
            DontDestroyOnLoad(gameObject);
            musicAudioSources.Add(gameObject.AddComponent<AudioSource>());
            musicAudioSources.Add(gameObject.AddComponent<AudioSource>());
            foreach (var audioSource in musicAudioSources)
            {
                audioSource.outputAudioMixerGroup = MixerGroup;
            }
        }

        public void PlayMusicClip(AudioClip musicClip, bool loop = true)
        {
            if (musicClip == null)
            {
                return;
            }
            var currentSourceIndex = activeAudioSourceIndex;

            //If the clip is already being played on the current audio source, no need to play anything
            if (currentSourceIndex >= 0 && musicClip == musicAudioSources[currentSourceIndex].clip)
            {
                return;
            }
            var nextSourceIndex = ++activeAudioSourceIndex % musicAudioSources.Count;

            // Now we will be definitely playing something. Set the new active index and the new played clip.
            activeAudioSourceIndex = nextSourceIndex;
            musicAudioSources[nextSourceIndex].clip = musicClip;
            musicAudioSources[nextSourceIndex].loop = loop;
            musicAudioSources[nextSourceIndex].Play();

            if (currentSourceIndex < 0)
            {
                // We weren't playing anything before, no need for a transition.
                return;
            }

            //If a transition is already happening, we stop it here to prevent our new Coroutine from competing
            if (activeTransition != null)
            {
                StopCoroutine(activeTransition);
            }

            activeTransition = VolumeTransition(TransitionDuration, currentSourceIndex, nextSourceIndex); 
            StartCoroutine(activeTransition);
        }

        private IEnumerator VolumeTransition(float transitionDuration, int fadeOutAudioSourceIndex, int fadeInAudioSourceIndex)
        {
            var volumeIncrement = 1 / transitionDuration * TransitionStepLength; // Each iteration takes 100 ms, so if duration is 1s, we want to gave increment of 0.1
            for (var currentFadeInTrackVolume = volumeIncrement; currentFadeInTrackVolume < 1; currentFadeInTrackVolume += volumeIncrement)
            {
                musicAudioSources[fadeOutAudioSourceIndex].volume = 1 - currentFadeInTrackVolume;
                musicAudioSources[fadeInAudioSourceIndex].volume = currentFadeInTrackVolume;

                yield return new WaitForSecondsRealtime(TransitionStepLength);
            }

            musicAudioSources[fadeInAudioSourceIndex].volume = 1;
            musicAudioSources[fadeOutAudioSourceIndex].Stop();

            activeTransition = null;
        }
    }
}
