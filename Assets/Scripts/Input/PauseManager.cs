using UnityEngine;

namespace Assets.Scripts.Input
{
    public class PauseManager : MonoBehaviour
    {
        // Update is called once per frame
        void Update()
        {
            if (UnityEngine.Input.GetKeyUp(KeyCode.Space))
            {
                ToggleGamePaused();
            }
        }

        void ToggleGamePaused()
        {
            Time.timeScale = Time.timeScale == 1 ? 0 : 1;
        }
    }
}
