using UnityEngine;

namespace Assets.Scripts.Input
{
    /// <summary>
    /// Manages all the different reasons why the game might be paused. Including pausing on spacebar press.
    /// </summary>
    public class PauseManager : MonoBehaviour
    {
        /// <summary>
        /// If true, the game is currently paused by some UI element appearing on the screen, e.g. skill description.
        /// </summary>
        public bool IsPausedByUi { get; set; }
        /// <summary>
        /// If true, the player paused the game by space bar.
        /// </summary>
        public bool IsPausedByPlayer { get; private set; }
        /// <summary>
        /// If true, the game is paused because the player is in main menu.
        /// </summary>
        public bool IsPausedByMenu { get; set; }
        /// <summary>
        /// If true, the game is paused for any reason.
        /// </summary>
        public bool IsPaused => IsPausedByUi || IsPausedByPlayer || IsPausedByMenu;

        /// <summary>
        /// Update is called every frame. 
        /// Detects the player pausing or unpausing the game by space bar.
        /// Sets the time scale to zero when paused, 1 when not paused.
        /// </summary>
        void Update()
        {
            if (UnityEngine.Input.GetKeyUp(KeyCode.Space) && !IsPausedByUi && !IsPausedByMenu)
            {
                IsPausedByPlayer = !IsPausedByPlayer;
            }

            Time.timeScale = IsPaused ? 0 : 1;
        }
    }
}
