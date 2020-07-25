using Assets.Scripts.Analytics;
using Assets.Scripts.Input;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// The component that shows and controls the ESC menu in game.
    /// </summary>
    public class InGameMenuController: MonoBehaviour
    {
        /// <summary>
        /// Text to be shown to ask the player if he is sure he wants to exit the game.
        /// </summary>
        [TextArea(3, 6)]
        public string ExitGameConfirmationString;
        /// <summary>
        /// Text to be shown to ask the player if he is sure he wants to revoke the marketing agreement.
        /// </summary>
        [TextArea(3, 6)]
        public string RevokeAgreementConfirmationString;
        /// <summary>
        /// The game object showing the screen menu, i.e. the buttons for exiting the game, revoking etc.
        /// </summary>
        public GameObject MenuObject;
        /// <summary>
        /// The game object showing the confirmation.
        /// </summary>
        public GameObject ConfirmationObject;
        /// <summary>
        /// The overlay over the entire screen.
        /// </summary>
        public GameObject OverlayObject;
        /// <summary>
        /// The UI element which shows the confirmation text.
        /// </summary>
        public Text ConfirmationText;
        /// <summary>
        /// In what state is the main menu right now.
        /// </summary>
        [HideInInspector]
        public InGameMenuState CurrentState;
        /// <summary>
        /// Component which controls the game being paused. Used because main menu pauses the game while visible.
        /// </summary>
        private PauseManager pauseManager;
        /// <summary>
        /// Called when the user presses the Yes confirmation button.
        /// Does the action that was being confirmed.
        /// </summary>
        public void ConfirmationYesPressed()
        {
            switch (CurrentState)
            {
                case InGameMenuState.ExitingGame:
                    Application.Quit();
                    break;
                case InGameMenuState.RevokingAgreement:
                    FindObjectOfType<AnalyticsService>().LogRevoke();
                    break;
                default:
                    UnityEngine.Debug.Assert(false, "Confirmation of menu pressed in invalid state");
                    break;
            }
        }
        /// <summary>
        /// Called when the user presses the No confirmation menu.
        /// Moves back to the main menu.
        /// </summary>
        public void ConfirmationNoPressed()
        {
            CurrentState = InGameMenuState.Menu;
        }
        /// <summary>
        /// Called when the user presses back in the menu. Hides the menu.
        /// </summary>
        public void MenuBackPressed()
        {
            CurrentState = InGameMenuState.Inactive;
        }
        /// <summary>
        /// Called when the user presses Revoke Agreement. Asks for confirmation. If confirmed, <see cref="ConfirmationYesPressed"/> will start the revocation.
        /// </summary>
        public void MenuRevokeAgreementPressed()
        {
            CurrentState = InGameMenuState.RevokingAgreement;
        }
        /// <summary>
        /// Called when the user presses Exit Game. Asks for confirmation. If confirmed, <see cref="ConfirmationYesPressed"/> will exit the game.
        /// </summary>
        public void MenuExitGamePressed()
        {
            CurrentState = InGameMenuState.ExitingGame;
        }
        /// <summary>
        /// Called before first update. Updates references to the dependencies.
        /// </summary>
        private void Start()
        {
            pauseManager = FindObjectOfType<PauseManager>();
        }
        /// <summary>
        /// Called every frame. Detects ESC press and ensures that the main menu UI matches the state of this object.
        /// </summary>
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
        /// <summary>
        /// Updates the <see cref="PauseManager.IsPausedByMenu"/>, tells it whether the menu needs the game to be pause right now.
        /// </summary>
        private void UpdateGamePaused()
        {
            bool isPaused = CurrentState != InGameMenuState.Inactive;
            if (pauseManager != null)
            {
                pauseManager.IsPausedByMenu = isPaused;
            }
            else
            {
                // To ensure this works in credits. Because the pause manager does not exist there.
                Time.timeScale = isPaused ? 0 : 1;
            }
        }
        /// <summary>
        /// If ESC is pressed, either show or hide the menu.
        /// </summary>
        private void DetectEscapePress()
        {
            if (UnityEngine.Input.GetKeyUp(KeyCode.Escape))
            {
                CurrentState = CurrentState == InGameMenuState.Inactive ? InGameMenuState.Menu : InGameMenuState.Inactive;
            }
        }
    }
    /// <summary>
    /// What should the menu be showing right now.
    /// </summary>
    public enum InGameMenuState
    {
        /// <summary>
        /// Menu is inactive and should not be shown at all.
        /// </summary>
        Inactive,
        /// <summary>
        /// Menu is being at its first page.
        /// </summary>
        Menu,
        /// <summary>
        /// Menu is asking for confirmation of the agreement revocation.
        /// </summary>
        RevokingAgreement,
        /// <summary>
        /// Menu is asking for confirmation of exiting the game.
        /// </summary>
        ExitingGame
    }
}
