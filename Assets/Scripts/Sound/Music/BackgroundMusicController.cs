using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Combat;
using Assets.Scripts.Extension;
using Assets.Scripts.GameFlow;
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
        private GameStateManager gameStateManager;
        private bool isInMainMenuMode;

        private void Start()
        {
            transitionManger = FindObjectOfType<MusicTransitionManger>();
            OnSceneFirstEntered();
            SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
        }

        private void Update()
        {
            if (isInMainMenuMode && FindObjectOfType<CombatantsManager>() != null)
            {
                // Bit hacky, basically once the level loads we want to turn the music on.
                OnSceneFirstEntered();
            }
        }

        private void GameStateManager_GameOver(object sender, EventArgs e)
        {
            transitionManger.PlayMusicClip(MusicClips.GameOverMusic.GetRandomElementOrDefault(), loop: false);
        }

        private void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
        {
            OnSceneFirstEntered();
        }

        private void OnSceneFirstEntered()
        {
            var combatantsManager = FindObjectOfType<CombatantsManager>();
            isInMainMenuMode = combatantsManager == null;
            if (isInMainMenuMode)
            {
                // Probably credits or main menu.
                // TODO: Differentiate between credits and main menu.
                transitionManger.PlayMusicClip(MusicClips.MainMenuMusic);
                return;
            }
            gameStateManager = FindObjectOfType<GameStateManager>();
            gameStateManager.GameOver += GameStateManager_GameOver;
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
