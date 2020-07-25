using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Sound.CharacterSounds
{
    /// <summary>
    /// Possible kinds of skills that can trigger a sound.
    /// </summary>
    public enum SkillVoiceType
    {
        /// <summary>
        /// The skill was a basic attack.
        /// </summary>
        BasicAttack,
        /// <summary>
        /// The skill was some normal skill the combatant uses.
        /// </summary>
        SkillNormal,
        /// <summary>
        /// For this skill the combatant has a different set of audio effects.
        /// </summary>
        SkillAlternate
    }
}
