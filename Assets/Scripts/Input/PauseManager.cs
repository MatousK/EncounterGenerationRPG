using UnityEngine;

namespace Assets.Scripts.Input
{
    public class PauseManager : MonoBehaviour
    {
        public bool IsPausedByUi { get; set; }
        public bool IsPausedByPlayer { get; private set; }

        public bool IsPaused => IsPausedByUi || IsPausedByPlayer;

        // Update is called once per frame
        void Update()
        {
            if (UnityEngine.Input.GetKeyUp(KeyCode.Space) && !IsPausedByUi)
            {
                IsPausedByPlayer = !IsPausedByPlayer;
            }

            Time.timeScale = IsPaused ? 0 : 1;
        }
    }
}
