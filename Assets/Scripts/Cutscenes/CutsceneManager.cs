using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Can play cutscenes. AI, camera and input classes are expected to do nothing while a cutscene is playing.
/// </summary>
public class CutsceneManager : MonoBehaviour
{
    Cutscene currentCutscene;
    /// <summary>
    /// Returns true if a cutscene is currently playing.
    /// </summary>
    public bool IsCutsceneActive
    {
        get
        {
            return currentCutscene != null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (currentCutscene?.IsCutsceneActive() == false)
        {
            currentCutscene.EndCutscene();
            Destroy(currentCutscene.gameObject);
            currentCutscene = null;
        }
    }
    /// <summary>
    /// Adds a cutscene of the specified type to this manager. 
    /// <paramref name="autoplay"> If true, cutscene should also play immediately.</paramref>>
    /// </summary>
    /// <typeparam name="T">The cutscene to play.</typeparam>
    public T InstantiateCutscene<T>(bool autoplay = false) where T: Cutscene
    {
        var gameObject = new GameObject("Generated cutscene");
        gameObject.AddComponent<T>();
        gameObject.transform.parent = transform;
        if (autoplay)
        {
            PlayCutscene(gameObject.GetComponent<T>());
        }
        return gameObject.GetComponent<T>();
    }
    /// <summary>
    /// Plays a specific cutscene. Will destroy the instance on cutscene completion.
    /// </summary>
    /// <param name="cutsceneToPlay">The cutscene to play.</param>
    public void PlayCutscene(Cutscene cutsceneToPlay)
    {
        currentCutscene = cutsceneToPlay;
        currentCutscene.StartCutscene();
    }
}
