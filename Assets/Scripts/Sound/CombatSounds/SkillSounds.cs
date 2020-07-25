using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Sound.CombatSounds
{
    /// <summary>
    /// List of sound effects that can be registered.
    /// </summary>
    [Serializable]
    public class SkillSounds
    {
        /// <summary>
        /// Type of skills for which these sounds should play.
        /// </summary>
        public CombatSoundType CombatSoundType;
        /// <summary>
        /// A random effects from this list will play when the skill animation starts.
        /// </summary>
        public List<AudioClip> AnimationStartAudioClips = new List<AudioClip>();
        /// <summary>
        /// A random effects from this list will play when the skill animation effect is applied, e.g. sword hitting the enemy.
        /// </summary>
        public List<AudioClip> AnimationApplyEffectAudioClips = new List<AudioClip>();
        /// <summary>
        /// A random effects from this list will play when the skill animation end.
        /// </summary>
        public List<AudioClip> AnimationEndAudioClips = new List<AudioClip>();
        /// <summary>
        /// A random effects from this list will play when the a projectile from the skill hits the target.
        /// </summary>
        public List<AudioClip> ProjectileHitAudioClips = new List<AudioClip>();
    }
}
