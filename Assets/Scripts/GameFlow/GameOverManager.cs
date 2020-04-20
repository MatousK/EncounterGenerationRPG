using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Combat;
using Assets.Scripts.Sound.Music;
using UnityEngine;

namespace Assets.Scripts.GameFlow
{
    class GameOverManager: MonoBehaviour
    {
        private CombatantsManager combatantsManager;
        /// <summary>
        /// Flag that ensures that we w
        /// </summary>
        private bool isGameOverInProgress = false;
        private GameStateManager gameStateManager;
        private void Start()
        {
            combatantsManager = FindObjectOfType<CombatantsManager>();
            gameStateManager = FindObjectOfType<GameStateManager>();
        }

        private void Update()
        {
            if (combatantsManager.PlayerCharacters.Any() &&
                !combatantsManager.GetPlayerCharacters(onlyAlive: true).Any())
            {
                // First condition checks that player characters were already spawned. Second checks if they are all dead.
                if (!isGameOverInProgress)
                {
                    gameStateManager.OnGameOver();
                    isGameOverInProgress = true;
                }
            }
            else
            {
                // Players not spawned or not dead, so not in game over state.
                isGameOverInProgress = false;
            }
        }
    }
}
