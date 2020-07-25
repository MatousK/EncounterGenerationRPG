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
    /// Takes care of playing music. There is only one of these in the game.
    /// It is always paired with the <see cref="MusicTransitionManger"/>, which sets the object to DontDestroyOnLoad.
    /// </summary>
    public class BackgroundMusicController : MonoBehaviour
    {
        /// <summary>
        /// List of all music clips the component can play in different situations.
        /// </summary>
        public GameMusicClips MusicClips;
        /// <summary>
        /// The component for transitioning between different music clips by one fading out and one fading in.
        /// </summary>
        private MusicTransitionManger transitionManger;
        /// <summary>
        /// Component which manages game over and game reloaded event, we use them to play game over music.
        /// </summary>
        private GameStateManager gameStateManager;
        /// <summary>
        /// Level loader which know which level is active right now. We use it to determine which music should play right now.
        /// </summary>
        private LevelLoader levelLoader;
        /// <summary>
        /// Called before first update. Finds instances of dependencies, subscribes to events and starts the first music. 
        /// </summary>
        private void Start()
        {
            levelLoader = FindObjectOfType<LevelLoader>();
            transitionManger = FindObjectOfType<MusicTransitionManger>();
            OnSceneFirstEntered();
            SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
        }
        /// <summary>
        /// Called every frame. Refreshes references to the scene objects, as the <see cref="gameStateManager"/> and <see cref="CombatantsManager"/> are not persistent,
        /// yet we are subscribed to their events.
        /// </summary>
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
        /// <summary>
        /// Plays the game over music.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Argumetns of the method.</param>
        private void GameStateManager_GameOver(object sender, EventArgs e)
        {
            transitionManger.PlayMusicClip(MusicClips.GameOverMusic.GetRandomElementOrDefault(), loop: false);
        }
        /// <summary>
        /// On scene transition, find out which music should play right now and play it.
        /// </summary>
        /// <param name="arg0">Ignored.</param>
        /// <param name="arg1">Ignored.</param>
        private void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
        {
            OnSceneFirstEntered();
        }
        /// <summary>
        /// When a scene is entered, select the music that should play and play it.
        /// </summary>
        private void OnSceneFirstEntered()
        {
            PlayBackgroundMusic();
        }
        /// <summary>
        /// Combat ended, don't play combat music anymore, start playing the background music once more.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Arguments of the event.</param>
        private void CombatantsManager_CombatOver(object sender, EventArgs e)
        {
            PlayBackgroundMusic();
        }
        /// <summary>
        /// Called when combat starts. Start playing the appropriate combat music.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Info about the combat which started.</param>
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
        /// <summary>
        /// Plays the background music based on which scene is active right now.
        /// </summary>
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
