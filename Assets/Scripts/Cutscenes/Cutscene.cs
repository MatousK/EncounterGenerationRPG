using UnityEngine;

namespace Assets.Scripts.Cutscenes
{
    /// <summary>
    /// Base class for all cutscenes that can be run in the game.
    /// </summary>
    public abstract class Cutscene: MonoBehaviour
    {
        /// <summary>
        /// Called by the CutsceneManager when this cutscene starts.
        /// </summary>
        public abstract void StartCutscene();
        /// <summary>
        /// The cutscene is presumed to be active while this method is returning true.
        /// </summary>
        /// <returns>True if the cutscene is active, otherwise false.</returns>
        public abstract bool IsCutsceneActive();
        /// <summary>
        /// Called by the cutscene manager before this cutscene is ended as a result of IsCutsceneActive returning true.
        /// </summary>
        public abstract void EndCutscene();
    }
}