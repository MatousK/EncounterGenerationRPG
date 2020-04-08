using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.GameFlow
{
    class GameStateManager: MonoBehaviour
    {
        public event EventHandler GameOver;
        public event EventHandler GameReloaded;
        public void OnGameOver()
        {
            GameOver?.Invoke(this, new EventArgs());
        }

        public void OnGameReloaded()
        {
            GameReloaded?.Invoke(this, new EventArgs());
        }
    }
}
