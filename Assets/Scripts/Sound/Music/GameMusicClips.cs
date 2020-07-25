using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Sound.Music
{
    /// <summary>
    /// Specifies music clips for different situations.
    /// </summary>
    [CreateAssetMenu(menuName = "Sounds/Music", fileName = "Music")]
    public class GameMusicClips: ScriptableObject
    {
        /// <summary>
        /// Possible background music when nothing interesting is happening.
        /// </summary>
        public List<AudioClip> IdleMusic = new List<AudioClip>();
        /// <summary>
        /// Combat music during normal fights.
        /// </summary>
        public List<AudioClip> CombatMusic = new List<AudioClip>();
        /// <summary>
        /// Combat music during boss fights.
        /// </summary>
        public List<AudioClip> BossFightMusic = new List<AudioClip>();
        /// <summary>
        /// Music that plays during game over screen.
        /// </summary>
        public List<AudioClip> GameOverMusic = new List<AudioClip>();
        /// <summary>
        /// Music that plays during the credits.
        /// </summary>
        public AudioClip CreditsMusic;
        /// <summary>
        /// Music that plays during the main menu.
        /// </summary>
        public AudioClip MainMenuMusic;
    }
}
