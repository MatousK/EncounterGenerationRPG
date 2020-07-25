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
    /// <summary>
    /// This component is responsible for loading the next level after the current one ends.
    /// It also provides information about the current level to the rest of the game.
    /// This class was originally meant to also support free play, where the player could play indefinitely. This mode would be unlocked after finishing the experiment.
    /// However, we did not have the time to implement this.
    /// This is a DontDestroyOnLoad object. It persists between scenes and there will only be one of them in any scene.
    /// </summary>
    class LevelLoader: MonoBehaviour
    {
        /// <summary>
        /// The ordered list of levels that make up story mode. When the Story Mode is started, this levels will be played one by one.
        /// </summary>
        public LevelDefinition[] StoryModeLevels;
        /// <summary>
        /// The level that should be played over and over during free play.
        /// </summary>
        public LevelDefinition FreePlayLevel;
        /// <summary>
        /// The party configuration that should be spawned at the start of this level. Which heroes should spawn and their attributes.
        /// </summary>
        public PartyConfiguration CurrentPartyConfiguration;
        /// <summary>
        /// The object to use as a level intro screen if a text and/or survey link should be shown.
        /// </summary>
        public GameObject LevelIntroScreenTemplate;
        /// <summary>
        /// If true, this object is about to be destroyed. 
        /// Needed because when this object calls Destroy on itself in Awake, it is not destroy immediately and other components might have problems with finding the one true instance of this class.
        /// </summary>
        [HideInInspector]
        public bool IsPendingKill;
        /// <summary>
        /// The level graph that should be used to generate the current level,
        /// </summary>
        [HideInInspector]
        public LevelGraph CurrentLevelGraph;
        /// <summary>
        /// The method for generating enemies that should be used in this level.
        /// </summary>
        public EncounterGenerationAlgorithmType CurrentEncounterGenerationAlgorithm;
        /// <summary>
        /// If true, then for the current level the matrix should be updated even for static encounters.
        /// </summary>
        [HideInInspector]
        public bool AdjustMatrixForStaticEncounters;
        /// <summary>
        /// If true, doors use an alternate set of colors to specify difficulty. Used for phase III of the experiment.
        /// </summary>
        [HideInInspector]
        public bool UseAlternateDoorColors;
        /// <summary>
        /// Which scene should be loaded for the current level.
        /// </summary>
        public SceneType CurrentSceneType;
        /// <summary>
        /// The component which knows about the experiment group of the current player.
        /// </summary>
        private AbTestingManager abTestingManager;
        /// <summary>
        /// The index of the story mode level the player is currently playing.
        /// </summary>
        private int currentStoryModeLevelIndex;
        /// <summary>
        /// If true, the player is currently playing the story mode.
        /// </summary>
        private bool isPlayingStoryMode;
        /// <summary>
        /// The animation to be played during transitions between levels - fade out and fade in one after another.
        /// </summary>
        private Animation animationComponent;
        /// <summary>
        /// The name of the scene that should be loaded once screen fades out.
        /// </summary>
        private string nextLevelSceneName;
        /// <summary>
        /// After the tutorial ends, we store the party stats here. We need them so we can start phase III with the same stats as phase II.
        /// </summary>
        private PartyConfiguration storedTutorialConfiguration;
        /// <summary>
        /// The component that can send analytics information to the backend.
        /// </summary>
        private AnalyticsService analyticsService;
        /// <summary>
        /// If true, the tutorial was already shown.
        /// We are supposed to show tutorial after the first level loads.
        /// Once this is false, we should not skip the tutorial any more.
        /// </summary>
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
        /// <summary>
        /// Start the story mode from the first level.
        /// </summary>
        public void StartStoryMode()
        {
            currentStoryModeLevelIndex = 0;
            isPlayingStoryMode = true;
            LoadLevelWithIntro(StoryModeLevels.First());
        }
        /// <summary>
        /// Start the free play.
        /// </summary>
        public void StartFreeMode()
        {
            isPlayingStoryMode = false;
            LoadLevelWithIntro(FreePlayLevel);
        }
        /// <summary>
        /// Show the credits for the game.
        /// </summary>
        public void OpenCredits()
        {
            CurrentSceneType = SceneType.Credits;
            SceneManager.LoadScene("Credits");
        }
        /// <summary>
        /// Show the main menu screen.
        /// </summary>
        public void OpenMainMenu()
        {
            CurrentSceneType = SceneType.MainMenu;
            SceneManager.LoadScene("MainMenu");
        }
        /// <summary>
        /// Load the next level. This should be called once the player finishes the current level.
        /// Will show the intro text and/or survey link if defined for the level.
        /// Otherwise it will just do the fade out/fade in transition.
        /// </summary>
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
        /// <summary>
        /// Loads the specified level, first showing its intro text/survey link if defined.
        /// </summary>
        /// <param name="level">The level to load.</param>
        public void LoadLevelWithIntro(LevelDefinition level)
        {
            // First, change attributes specific to the current level to the ones from the level to load.
            var experimentGroup = abTestingManager.CurrentExperimentGroup;
            var experimentConfiguration =
                level.ExperimentGroupConfigurations.First(config => config.ExperimentGroup == experimentGroup);
            CurrentEncounterGenerationAlgorithm = experimentConfiguration.Algorithm;
            AdjustMatrixForStaticEncounters = level.AdjustMatrixForStaticEncounters;
            UseAlternateDoorColors = level.UseAlternateDoorColors;
            var surveyLink = experimentConfiguration.SurveyLink;
            // If there is an intro text and/or survey, show it. Otherwise just load the next level.
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
        /// <summary>
        /// Intro done or there never was. Fade out the screen, change the level and fade in.
        /// LoadLevelWithIntro should always be called before this.
        /// </summary>
        /// <param name="level"></param>
        public void LoadLevel(LevelDefinition level)
        {
            // If loading next floor, store the hero attributes.
            var existingHeroes = FindObjectsOfType<Hero>();
            CurrentPartyConfiguration = existingHeroes.Any() ? new PartyConfiguration(existingHeroes.ToArray()) : null;
            if (storedTutorialConfiguration == null)
            {
                // If we have not stored the after tutorial configuration, this level must have been the tutorial.
                storedTutorialConfiguration = CurrentPartyConfiguration;
            }

            if (level.ShouldRestoreAfterTutorialStats && storedTutorialConfiguration != null)
            {
                CurrentPartyConfiguration = storedTutorialConfiguration;
            }
            CurrentSceneType = level.Type;
            CurrentLevelGraph = level.PossibleLevelGraphs.GetRandomElementOrDefault();
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
            // Start the fade animation. It will fade out and then fade in, it is a UI screen over everything.
            // Once the fade out part ends, change the level. Once the level is loaded, fade back in.
            animationComponent.Play(PlayMode.StopAll);
        }
        /// <summary>
        /// Called by the level transition animation when the fade out part of the animation is done.
        /// Change the scene.
        /// </summary>
        public void FadeOutDone()
        {
            SceneManager.LoadScene(nextLevelSceneName);
        }
        /// <summary>
        /// Called once the level is completely loaded. 
        /// Starts the tutorial if we have not started it yet.
        /// </summary>
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
