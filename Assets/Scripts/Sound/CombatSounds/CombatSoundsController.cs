using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Extension;
using GeneralAlgorithms.Algorithms.Common;
using UnityEngine;

namespace Assets.Scripts.Sound.CombatSounds
{
    public class CombatSoundsController: MonoBehaviour
    {
        public List<SkillSounds> RegisteredSounds = new List<SkillSounds>();
        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public void AnimationStarted(CombatSoundType skillSoundType)
        {
            PlaySound(skillSoundType, skillSounds => skillSounds.AnimationStartAudioClips);
        }
        public void AnimationEffectApplied(CombatSoundType skillSoundType)
        {
            PlaySound(skillSoundType, skillSounds => skillSounds.AnimationApplyEffectAudioClips);
        }
        public void AnimationEnded(CombatSoundType skillSoundType)
        {
            PlaySound(skillSoundType, skillSounds => skillSounds.AnimationEndAudioClips);
        }
        public void ProjectileHit(CombatSoundType skillSoundType)
        {
            PlaySound(skillSoundType, skillSounds => skillSounds.ProjectileHitAudioClips);
        }

        private void PlaySound(CombatSoundType soundType, Func<SkillSounds, List<AudioClip>> soundListSelector)
        {
            var relevantSkillSounds = GetSkillSounds(soundType);
            if (relevantSkillSounds == null)
            {
                return;
            }

            var toPlay = soundListSelector(relevantSkillSounds).GetWeightedRandomElementOrDefault(clip => 1);
            if (toPlay != null && audioSource != null)
            {
                audioSource.PlayOneShot(toPlay);
            }
        }

        private SkillSounds GetSkillSounds(CombatSoundType soundType)
        {
            return RegisteredSounds.FirstOrDefault(sound => sound.CombatSoundType == soundType);
        }
    }
}
