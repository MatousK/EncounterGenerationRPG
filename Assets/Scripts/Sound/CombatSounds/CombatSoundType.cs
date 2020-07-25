using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Sound.CombatSounds
{
    /// <summary>
    /// Describes what kind of a sound should be played when using this skill.
    /// </summary>
    public enum CombatSoundType
    {
        /// <summary>
        /// No sound effects should play.
        /// </summary>
        None,
        /// <summary>
        /// Sounds effects for sword attacks
        /// </summary>
        Sword,
        /// <summary>
        /// Sounds effects for crushing attacks
        /// </summary>
        Crushing,
        /// <summary>
        /// Sounds effects for hand to hand attacks
        /// </summary>
        HandToHand,
        /// <summary>
        /// Sounds effects for bow skills.
        /// </summary>
        Bow,
        /// <summary>
        /// Sounds effects for fireball skills.
        /// </summary>
        Fireball,
        /// <summary>
        /// Sounds effects for healing skills.
        /// </summary>
        MagicHeal,
        /// <summary>
        /// Sounds effects for the sleep skill.
        /// </summary>
        Sleep,
        /// <summary>
        /// Sounds effects for dagger attacks.
        /// </summary>
        Dagger
    }
}
