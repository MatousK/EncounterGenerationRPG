using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Sound.CombatSounds
{
    [Serializable]
    public class SkillSounds
    {
        public CombatSoundType CombatSoundType;
        public List<AudioClip> AnimationStartAudioClips = new List<AudioClip>();
        public List<AudioClip> AnimationApplyEffectAudioClips = new List<AudioClip>();
        public List<AudioClip> AnimationEndAudioClips = new List<AudioClip>();
        public List<AudioClip> ProjectileHitAudioClips = new List<AudioClip>();
    }
}
