using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Sound.CharacterSounds
{
    /// <summary>
    /// Types of orders that can be given to a hero.
    /// </summary>
    public enum VoiceOrderType
    {
        /// <summary>
        /// Hero is ordered to attack his target.
        /// </summary>
        Attack,
        /// <summary>
        /// Hero is ordered to move somewhere.
        /// </summary>
        Move,
        /// <summary>
        /// The hero is ordered to use his friendly skill.
        /// </summary>
        FriendlySkill,
        /// <summary>
        /// The hero is ordered to use his enemy skill.
        /// </summary>
        EnemySkill,
        /// <summary>
        /// The hero is ordered to use his personal skill.
        /// </summary>
        SelfSkill
    }
}
