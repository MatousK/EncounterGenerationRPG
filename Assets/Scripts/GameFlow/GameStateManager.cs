using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.GameFlow
{
    /// <summary>
    /// This component has events notifying the rest of the game about changes in the game state, whether the game is over or whether it was reloaded.
    /// Does not raise the events on its own, other components in the game must do that.
    /// </summary>
    class GameStateManager: MonoBehaviour
    {
        /// <summary>
        /// The event raised when the party is defeated.
        /// </summary>
        public event EventHandler GameOver;
        /// <summary>
        /// The event raised when the game is reloaded.
        /// </summary>
        public event EventHandler GameReloaded;
        /// <summary>
        /// Call when the game is over to raise the <see cref="GameOver"/> event.
        /// </summary>
        public void OnGameOver()
        {
            GameOver?.Invoke(this, new EventArgs());
        }
        /// <summary>
        /// Call when the game is over to raise the <see cref="GameReloaded"/> event.
        /// </summary>
        public void OnGameReloaded()
        {
            GameReloaded?.Invoke(this, new EventArgs());
        }
    }
}
