using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Sound.CharacterSounds
{
    /// <summary>
    /// All the possible lines a combatant might say.
    /// </summary>
    [CreateAssetMenu(menuName = "Sounds/Character Voices", fileName = "Character Voices")]
    public class CharacterVoiceSounds: ScriptableObject
    {
        /// <summary>
        /// Plays when the combatant is selected.
        /// </summary>
        public List<AudioClip> SelectedSounds = new List<AudioClip>();
        /// <summary>
        /// Plays when the combatant is ordered to move elsewhere
        /// </summary>
        public List<AudioClip> MoveOrderSounds = new List<AudioClip>();
        /// <summary>
        /// Plays when the combatant is ordered to attack someone.
        /// </summary>
        public List<AudioClip> AttackOrderSounds = new List<AudioClip>();
        /// <summary>
        /// Plays when the combatant is ordered to do the enemy skill.
        /// </summary>
        public List<AudioClip> EnemySkillOrderSounds = new List<AudioClip>();
        /// <summary>
        /// Plays when the combatant is ordered to do the personal skill.
        /// </summary>
        public List<AudioClip> PersonalSkillOrderSounds = new List<AudioClip>();
        /// <summary>
        /// Plays when the combatant is ordered to do the friendly skill.
        /// </summary>
        public List<AudioClip> FriendlySkillOrderSounds = new List<AudioClip>();
        /// <summary>
        /// Plays when the combatant uses a skill not marked as an alternate skill. 
        /// </summary>
        public List<AudioClip> SkillUsedSounds = new List<AudioClip>();
        /// <summary>
        /// Plays when the combatant uses a skill marked as an alternate skill.
        /// </summary>
        public List<AudioClip> AlternateSkillUsedSounds = new List<AudioClip>();
        /// <summary>
        /// Plays when the effect of this skill is applied
        /// </summary>
        public List<AudioClip> SkillEffectAppliedSounds = new List<AudioClip>();
        /// <summary>
        /// Plays when the effect is applied of a skill marked as an alternate skill.
        /// </summary>
        public List<AudioClip> AlternateEffectAppliedSounds = new List<AudioClip>();
        /// <summary>
        /// Plays when the hero is healed.
        /// </summary>
        public List<AudioClip> HealedSounds = new List<AudioClip>();
        /// <summary>
        /// Plays when the combatant starts an attack.
        /// </summary>
        public List<AudioClip> AttackGrunts = new List<AudioClip>();
        /// <summary>
        /// Plays when a combatant receives minor damage.
        /// </summary>
        public List<AudioClip> MinorDamageGrunts = new List<AudioClip>();
        /// <summary>
        /// Plays when a combatant receives major damage.
        /// </summary>
        public List<AudioClip> MajorDamageGrunts = new List<AudioClip>();
        /// <summary>
        /// Plays when a hero looses all HP and starts using his Max hp.
        /// </summary>
        public List<AudioClip> HeroStartsLosingMaxHpSounds = new List<AudioClip>();
        /// <summary>
        /// Plays when a combatant dies.
        /// </summary>
        public List<AudioClip> CombatantDiedSounds = new List<AudioClip>();
        /// <summary>
        /// Can play when the fight ends
        /// </summary>
        public List<AudioClip> FightOverSounds = new List<AudioClip>();
    }
}
