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
        private LevelLoader levelLoader;

        private void Start()
        {
            levelLoader = FindObjectOfType<LevelLoader>();
            transitionManger = FindObjectOfType<MusicTransitionManger>();
            OnSceneFirstEntered();
            SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
        }

        private void Update()
        {
            if (levelLoader.CurrentSceneType == SceneType.DungeonLevel && gameStateManager == null)
            {
                // Hacky way to link up with combatants and game state manager once they're loaded.
                gameStateManager = FindObjectOfType<GameStateManager>();
                if (gameStateManager == null)
                {
                    return;
                }
                var combatantsManager = FindObjectOfType<CombatantsManager>();
                gameStateManager = FindObjectOfType<GameStateManager>();
                gameStateManager.GameOver += GameStateManager_GameOver;
                combatantsManager.CombatStarted += CombatantsManager_CombatStarted;
                combatantsManager.CombatOver += CombatantsManager_CombatOver;
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
            PlayBackgroundMusic();
        }

        private void CombatantsManager_CombatOver(object sender, EventArgs e)
        {
            PlayBackgroundMusic();
        }

        private void CombatantsManager_CombatStarted(object sender, CombatStartedEventArgs e)
        {
            if (e.IsBossFight)
            {
                transitionManger.PlayMusicClip(MusicClips.BossFightMusic.GetRandomElementOrDefault());
            }
            else
            {
                transitionManger.PlayMusicClip(MusicClips.CombatMusic.GetRandomElementOrDefault());
            }
        }

        private void PlayBackgroundMusic()
        {
            switch (levelLoader.CurrentSceneType)
            {
                case SceneType.MainMenu:
                    transitionManger.PlayMusicClip(MusicClips.MainMenuMusic);
                    break;
                case SceneType.DungeonLevel:
                    transitionManger.PlayMusicClip(MusicClips.IdleMusic.GetRandomElementOrDefault());
                    break;
                case SceneType.Credits:
                    transitionManger.PlayMusicClip(MusicClips.CreditsMusic);
                    break;
                default:
                    UnityEngine.Debug.Assert(false, "Unknown scene type, cannot play music.");
                    break;
            }
        }
    }
}
