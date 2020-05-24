using System.Linq;
using Assets.ProceduralLevelGenerator.Scripts.Data.Graphs;
using Assets.Scripts.Analytics;
using Assets.Scripts.Combat;
using Assets.Scripts.CombatSimulator;
using Assets.Scripts.Experiment;
using Assets.Scripts.Extension;
using Assets.Scripts.Tutorial;
using Assets.Scripts.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.GameFlow
{
    class LevelLoader: MonoBehaviour
    {
        public LevelDefinition[] StoryModeLevels;

        public LevelDefinition FreePlayLevel;
        public PartyConfiguration CurrentPartyConfiguration;
        public GameObject LevelIntroScreenTemplate;
        [HideInInspector]
        public bool IsPendingKill;
        [HideInInspector]
        public LevelGraph CurrentLevelGraph;

        public EncounterGenerationAlgorithmType CurrentEncounterGenerationAlgorithm;

        [HideInInspector]
        public bool AdjustMatrixForStaticEncounters;
        /// <summary>
        /// If true, doors use an alternate set of colors to specify difficulty. Used for phase II of the experiment.
        /// </summary>
        [HideInInspector]
        public bool UseAlternateDoorColors;

        public SceneType CurrentSceneType;
        private AbTestingManager abTestingManager;
        private int currentStoryModeLevelIndex;
        private bool isPlayingStoryMode;
        private Animation animationComponent;
        private string nextLevelSceneName;
        /// <summary>
        /// After the tutorial, we want to save the party configuration and restore it later, so we can start after the tutorial.
        /// </summary>
        private PartyConfiguration storedTutorialConfiguration;
        private AnalyticsService analyticsService;

        private bool didShowTutorial;
        

        private void Awake()
        {
            if (FindObjectsOfType<LevelLoader>().Length > 1)
            {
                Destroy(gameObject);
                IsPendingKill = true;
            }
            DontDestroyOnLoad(gameObject);
            animationComponent = GetComponent<Animation>();
        }

        private void Start()
        {
            abTestingManager = FindObjectsOfType<AbTestingManager>().First(testingManager => !testingManager.IsPendingKill);
            analyticsService = FindObjectsOfType<AnalyticsService>().First(analyticsService => !analyticsService.IsPendingKill);
        }

        public void StartStoryMode()
        {
            currentStoryModeLevelIndex = 0;
            isPlayingStoryMode = true;
            LoadLevelWithIntro(StoryModeLevels.First());
        }

        public void StartFreeMode()
        {
            isPlayingStoryMode = false;
            LoadLevelWithIntro(FreePlayLevel);
        }

        public void OpenCredits()
        {
            CurrentSceneType = SceneType.Credits;
            SceneManager.LoadScene("Credits");
        }

        public void OpenMainMenu()
        {
            CurrentSceneType = SceneType.MainMenu;
            SceneManager.LoadScene("MainMenu");
        }

        public void LoadNextLevel()
        {
            if (isPlayingStoryMode)
            {
                if (++currentStoryModeLevelIndex >= StoryModeLevels.Length)
                {
                    UnityEngine.Debug.Assert(false,
                        "We should always end up in main menu and never call LoadNextLevel from there");
                }
                else
                {
                    analyticsService.LevelLoadStart(currentStoryModeLevelIndex);
                    LoadLevelWithIntro(StoryModeLevels[currentStoryModeLevelIndex]);
                }
            }
            else
            {
                LoadLevelWithIntro(FreePlayLevel);
            }
        }
        public void LoadLevelWithIntro(LevelDefinition level)
        {
            var experimentGroup = abTestingManager.CurrentExperimentGroup;
            var experimentConfiguration =
                level.ExperimentGroupConfigurations.First(config => config.ExperimentGroup == experimentGroup);
            CurrentEncounterGenerationAlgorithm = experimentConfiguration.Algorithm;
            AdjustMatrixForStaticEncounters = level.AdjustMatrixForStaticEncounters;
            UseAlternateDoorColors = level.UseAlternateDoorColors;
            var surveyLink = experimentConfiguration.SurveyLink;
            if (level.IntroTexts?.Any() == true || !string.IsNullOrEmpty(surveyLink))
            {
                var introScreenObject = Instantiate(LevelIntroScreenTemplate);
                var levelIntro = introScreenObject.GetComponentInChildren<TypewriterWithSurveyScreen>();
                levelIntro.SurveyLink = surveyLink;
                levelIntro.LevelDefinition = level;
            }
            else
            {
                LoadLevel(level);
            }
        }

        public void LoadLevel(LevelDefinition level)
        {
            // If loading next floor, store the hero attributes.
            var existingHeroes = FindObjectsOfType<Hero>();
            CurrentPartyConfiguration = existingHeroes.Any() ? new PartyConfiguration(existingHeroes.ToArray()) : null;
            if (storedTutorialConfiguration == null)
            {
                storedTutorialConfiguration = CurrentPartyConfiguration;
            }

            if (level.ShouldRestoreAfterTutorialStats && storedTutorialConfiguration != null)
            {
                CurrentPartyConfiguration = storedTutorialConfiguration;
            }
            CurrentSceneType = level.Type;
            switch (level.Type)
            {
                case SceneType.DungeonLevel:
                    nextLevelSceneName = "DungeonScene";
                    break;
                case SceneType.Credits:
                    nextLevelSceneName = "Credits";
                    break;
                case SceneType.MainMenu:
                    nextLevelSceneName = "MainMenu";
                    break;
            }
            CurrentLevelGraph = level.PossibleLevelGraphs.GetRandomElementOrDefault();
            animationComponent.Play(PlayMode.StopAll);
        }

        public void FadeOutDone()
        {
            SceneManager.LoadScene(nextLevelSceneName);
        }

        public void LevelLoadComplete()
        {
            analyticsService.LevelLoadEnd(currentStoryModeLevelIndex);
            var tutorialController = FindObjectOfType<TutorialController>();
            if (tutorialController != null && !didShowTutorial)
            {
                didShowTutorial = true;
                tutorialController.StartTutorial();
            }
        }
    }
}
