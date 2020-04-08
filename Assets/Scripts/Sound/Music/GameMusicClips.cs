using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Sound.Music
{
    [CreateAssetMenu(menuName = "Sounds/Music", fileName = "Music")]
    public class GameMusicClips: ScriptableObject
    {
        public List<AudioClip> IdleMusic = new List<AudioClip>();
        public List<AudioClip> CombatMusic = new List<AudioClip>();
        public List<AudioClip> BossFightMusic = new List<AudioClip>();
        public List<AudioClip> GameOverMusic = new List<AudioClip>();
        public AudioClip CreditsMusic;
        public AudioClip MainMenuMusic;
    }
}
