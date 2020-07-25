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
    /// <summary>
    /// This component listens checks player health every frame.
    /// If the party is dead, raises the Game Over event on the <see cref="GameStateManager"/>. 
    /// Raises it only once per wipe.
    /// </summary>
    class GameOverManager: MonoBehaviour
    {
        /// <summary>
        /// The component which knows about the state of all combatants in the game, including heroes and whether they are alive.
        /// </summary>
        private CombatantsManager combatantsManager;
        /// <summary>
        /// Flag that ensures that we do not raise the game over event multiple times. 
        /// </summary>
        private bool isGameOverInProgress = false;
        /// <summary>
        /// The component which notifies the rest of the objects in the game about changes in the current game state.
        /// That is the class on which we call the Game Over event.
        /// </summary>
        private GameStateManager gameStateManager;
        /// <summary>
        /// Called before the first frame, initializes references to dependencies.
        /// </summary>
        private void Start()
        {
            combatantsManager = FindObjectOfType<CombatantsManager>();
            gameStateManager = FindObjectOfType<GameStateManager>();
        }
        /// <summary>
        /// Called every frame. If the party is dead, call game over.
        /// </summary>
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
