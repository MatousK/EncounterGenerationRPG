using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Combat;
using Assets.Scripts.Extension;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Sound.Music
{
    /// <summary>
    /// Takes care of playing music. Will persist between scenes.
    /// </summary>
    public class BackgroundMusicController : MonoBehaviour
    {
        public GameMusicClips MusicClips;
        private MusicTransitionManger transitionManger;

        private void Start()
        {
            transitionManger = FindObjectOfType<MusicTransitionManger>();
            OnSceneFirstEntered();
            SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
        }

        private void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
        {
            OnSceneFirstEntered();
        }

        private void OnSceneFirstEntered()
        {
            var combatantsManager = FindObjectOfType<CombatantsManager>();
            if (combatantsManager == null)
            {
                // Probably credits or main menu.
                // TODO: Create an object that will define main menu and credits song, place it here.
                return;
            }
            transitionManger.PlayMusicClip(MusicClips.IdleMusic.GetRandomElementOrDefault());
            combatantsManager.CombatStarted += CombatantsManager_CombatStarted;
            combatantsManager.CombatOver += CombatantsManager_CombatOver;
        }

        private void CombatantsManager_CombatOver(object sender, EventArgs e)
        {
            transitionManger.PlayMusicClip(MusicClips.IdleMusic.GetRandomElementOrDefault());
        }

        private void CombatantsManager_CombatStarted(object sender, EventArgs e)
        {
            // TODO: Play bossfight music if in bossfight.
            transitionManger.PlayMusicClip(MusicClips.CombatMusic.GetRandomElementOrDefault());
        }
    }
}
