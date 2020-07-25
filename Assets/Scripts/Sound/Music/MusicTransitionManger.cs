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
    /// <summary>
    /// Class for managing fade in/fade out transitions when switching from one BGM track to another.
    /// Has two audio sources for playing the tracks. When music is changed, the inactive audio source starts playing,
    /// and increases in volume, while the formally active one decreases in volume until it goes silent, then it turns itself off.
    /// </summary>
    public class MusicTransitionManger: MonoBehaviour
    {
        /// <summary>
        /// How long should the transition be.
        /// </summary>
        public float TransitionDuration = 1f;
        /// <summary>
        /// The mixer group to which both audio tracks belong to. Used to control BGM volume.
        /// </summary>
        public AudioMixerGroup MixerGroup;
        /// <summary>
        /// The audio sources for each of the tracks fading in and out.
        /// </summary>
        private readonly List<AudioSource> musicAudioSources = new List<AudioSource>();
        /// <summary>
        /// The audio source that is playing the current background music.
        /// </summary>
        private int activeAudioSourceIndex = -1;
        /// <summary>
        ///  During the fade effect, this variable specifies how many seconds will elapse before changing the volume.
        /// </summary>
        private const float TransitionStepLength = 0.1f;
        /// <summary>
        /// The current transition being executed from one BGM to the next.
        /// </summary>
        IEnumerator activeTransition;
        /// <summary>
        /// Ensures there is only one persistent of this class available. Also adds the audio sources to this class.
        /// </summary>
        void Awake()
        {
            var musicControllers = FindObjectsOfType<MusicTransitionManger>();
            if (musicControllers.Length > 1)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
            musicAudioSources.Add(gameObject.AddComponent<AudioSource>());
            musicAudioSources.Add(gameObject.AddComponent<AudioSource>());
            foreach (var audioSource in musicAudioSources)
            {
                audioSource.outputAudioMixerGroup = MixerGroup;
            }
        }
        /// <summary>
        /// Plays the specified audio clip. With the fade transition
        /// </summary>
        /// <param name="musicClip">The music that should play.</param>
        /// <param name="loop">If true, the music should loop forever.</param>
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
        /// <summary>
        /// The coroutine which slowly modifies the volume of each audio source until one fades in and the other fades out completely.
        /// </summary>
        /// <param name="transitionDuration"></param>
        /// <param name="fadeOutAudioSourceIndex"></param>
        /// <param name="fadeInAudioSourceIndex"></param>
        /// <returns></returns>
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
