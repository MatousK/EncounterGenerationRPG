using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.ProceduralLevelGenerator.Scripts.Data.Graphs;
using Assets.Scripts.Combat;
using Assets.Scripts.CombatSimulator;
using Assets.Scripts.Extension;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.GameFlow
{
    class LevelLoader: MonoBehaviour
    {
        public LevelDefinition[] StoryModeLevels;

        public LevelDefinition FreePlayLevel;
        public PartyConfiguration CurrentPartyConfiguration;
        public LevelGraph CurrentLevelGraph;
        private int currentStoryModeLevelIndex;
        private bool isPlayingStoryMode;
        private Animation animationComponent;
        private string nextLevelSceneName;
        

        private void Awake()
        {
            if (FindObjectsOfType<LevelLoader>().Length > 1)
            {
                Destroy(gameObject);
            }
            DontDestroyOnLoad(gameObject);
            animationComponent = GetComponent<Animation>();
        }

        public void StartStoryMode()
        {
            currentStoryModeLevelIndex = 0;
            isPlayingStoryMode = true;
            LoadLevel(StoryModeLevels.First());
        }

        public void StartFreeMode()
        {
            isPlayingStoryMode = false;
            LoadLevel(FreePlayLevel);
        }

        public void LoadNextLevel()
        {
            if (isPlayingStoryMode)
            {
                if (++currentStoryModeLevelIndex >= StoryModeLevels.Length)
                {
                    ShowVictoryScreen();
                }
                else
                {
                    LoadLevel(StoryModeLevels[currentStoryModeLevelIndex]);
                }
            }
            else
            {
                LoadLevel(FreePlayLevel);
            }
        }

        public void ShowVictoryScreen()
        {

        }

        public void LoadLevel(LevelDefinition level)
        {
            // If loading next floor, store the hero attributes.
            var existingHeroes = FindObjectsOfType<Hero>();
            if (existingHeroes.Any())
            {
                CurrentPartyConfiguration = new PartyConfiguration(existingHeroes.ToArray());
            }
            else
            {
                CurrentPartyConfiguration = null;
            }
            nextLevelSceneName = "DungeonScene";
            CurrentLevelGraph = level.PossibleLevelGraphs.GetRandomElementOrDefault();
            animationComponent.Play();
        }

        public void FadeOutDone()
        {
            SceneManager.LoadScene(nextLevelSceneName);
        }
    }
}
