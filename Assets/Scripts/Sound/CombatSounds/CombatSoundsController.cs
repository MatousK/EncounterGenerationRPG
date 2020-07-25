using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Extension;
using GeneralAlgorithms.Algorithms.Common;
using UnityEngine;

namespace Assets.Scripts.Sound.CombatSounds
{
    /// <summary>
    /// The component which has registered sounds for possible sound effects and which can play them as required.
    /// </summary>
    public class CombatSoundsController: MonoBehaviour
    {
        /// <summary>
        /// All the combat sound effects this controller knows about.
        /// </summary>
        public List<SkillSounds> RegisteredSounds = new List<SkillSounds>();
        /// <summary>
        /// Audio source used to play the sound effects.
        /// </summary>
        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }
        /// <summary>
        /// Called when an animation of a skill has started. If we have the appropriate sound effect registered, play it.
        /// </summary>
        /// <param name="skillSoundType">The type of skill being used.</param>
        public void AnimationStarted(CombatSoundType skillSoundType)
        {
            PlaySound(skillSoundType, skillSounds => skillSounds.AnimationStartAudioClips);
        }
        /// <summary>
        /// Called when the effect of the animation is applied(e.g. an attack connected and hit an enemy). If we have the appropriate sound effect registered, play it.
        /// </summary>
        /// <param name="skillSoundType">The type of skill being used.</param>
        public void AnimationEffectApplied(CombatSoundType skillSoundType)
        {
            PlaySound(skillSoundType, skillSounds => skillSounds.AnimationApplyEffectAudioClips);
        }
        /// <summary>
        /// Called when an animation of a skill has ended. If we have the appropriate sound effect registered, play it.
        /// </summary>
        /// <param name="skillSoundType">The type of skill being used.</param>
        public void AnimationEnded(CombatSoundType skillSoundType)
        {
            PlaySound(skillSoundType, skillSounds => skillSounds.AnimationEndAudioClips);
        }
        /// <summary>
        /// Called a projectile from some skill hit a target. If we have the appropriate sound effect registered, play it.
        /// </summary>
        /// <param name="skillSoundType">The type of skill which launched the projectile.</param>
        public void ProjectileHit(CombatSoundType skillSoundType)
        {
            PlaySound(skillSoundType, skillSounds => skillSounds.ProjectileHitAudioClips);
        }
        /// <summary>
        /// Plays the appropriate sound effect for the parameters if registered.
        /// </summary>
        /// <param name="soundType">Type of a skill that launched the effect.</param>
        /// <param name="soundListSelector">The selector which will retrieve the appropriate list of sound effects from <see cref="SkillSounds"/>.</param>
        private void PlaySound(CombatSoundType soundType, Func<SkillSounds, List<AudioClip>> soundListSelector)
        {
            var relevantSkillSounds = GetSkillSounds(soundType);
            if (relevantSkillSounds == null)
            {
                // No sounds found.
                return;
            }
            // Sound effects found! Play a random sound effect.
            var toPlay = soundListSelector(relevantSkillSounds).GetRandomElementOrDefault();
            if (toPlay != null && audioSource != null)
            {
                audioSource.PlayOneShot(toPlay);
            }
        }
        /// <summary>
        /// Retrieve skill sounds registered to the specified type of sound effects,
        /// </summary>
        /// <param name="soundType">The type of sound effects.</param>
        /// <returns>The sound effects registered for the effect type, or null if none are registered.</returns>
        private SkillSounds GetSkillSounds(CombatSoundType soundType)
        {
            return RegisteredSounds.FirstOrDefault(sound => sound.CombatSoundType == soundType);
        }
    }
}
