using Assets.Scripts.Analytics;
using Assets.Scripts.Input;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class InGameMenuController: MonoBehaviour
    {
        [TextArea(3, 6)]
        public string ExitGameConfirmationString;
        [TextArea(3, 6)]
        public string RevokeAgreementConfirmationString;
        public GameObject MenuObject;
        public GameObject ConfirmationObject;
        public GameObject OverlayObject;
        public Text ConfirmationText;
        [HideInInspector]
        public InGameMenuState CurrentState;
        private PauseManager pauseManager;

        public void ConfirmationYesPressed()
        {
            switch (CurrentState)
            {
                case InGameMenuState.ExitingGame:
                    Application.Quit();
                    break;
                case InGameMenuState.RevokingAgreement:
                    FindObjectOfType<AnalyticsService>().LogRevokeAndExit();
                    break;
                default:
                    UnityEngine.Debug.Assert(false, "Confirmation of menu pressed in invalid state");
                    break;
            }
        }
        
        public void ConfirmationNoPressed()
        {
            CurrentState = InGameMenuState.Menu;
        }

        public void MenuBackPressed()
        {
            CurrentState = InGameMenuState.Inactive;
        }

        public void MenuRevokeAgreementPressed()
        {
            CurrentState = InGameMenuState.RevokingAgreement;
        }

        public void MenuExitGamePressed()
        {
            CurrentState = InGameMenuState.ExitingGame;
        }

        private void Start()
        {
            pauseManager = FindObjectOfType<PauseManager>();
        }

        private void Update()
        {
            DetectEscapePress();
            OverlayObject.SetActive(CurrentState != InGameMenuState.Inactive);
            ConfirmationObject.SetActive(CurrentState == InGameMenuState.RevokingAgreement || CurrentState == InGameMenuState.ExitingGame);
            MenuObject.SetActive(CurrentState == InGameMenuState.Menu);
            switch (CurrentState)
            {
                case InGameMenuState.RevokingAgreement:
                    ConfirmationText.text = RevokeAgreementConfirmationString;
                    break;
                case InGameMenuState.ExitingGame:
                    ConfirmationText.text = ExitGameConfirmationString;
                    break;
                default:
                    ConfirmationText.text = "You should never see this";
                    break;
            }
            UpdateGamePaused();
        }

        private void UpdateGamePaused()
        {
            bool isPaused = CurrentState != InGameMenuState.Inactive;
            if (pauseManager != null)
            {
                pauseManager.IsPausedByMenu = isPaused;
            }
            else
            {
                // To ensure this works in credits.
                Time.timeScale = isPaused ? 0 : 1;
            }
        }

        private void DetectEscapePress()
        {
            if (UnityEngine.Input.GetKeyUp(KeyCode.Escape))
            {
                CurrentState = CurrentState == InGameMenuState.Inactive ? InGameMenuState.Menu : InGameMenuState.Inactive;
            }
        }
    }

    public enum InGameMenuState
    {
        Inactive,
        Menu,
        RevokingAgreement,
        ExitingGame
    }
}
