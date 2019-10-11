using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            ToggleGamePaused();
        }
    }

    void ToggleGamePaused()
    {
        Time.timeScale = Time.timeScale == 1 ? 0 : 1;
    }
}
