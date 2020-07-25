using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Combat;
using Assets.Scripts.GameFlow;

namespace Assets.Scripts.Tutorial
{
    /// <summary>
    /// <inheritdoc/>
    /// This is not a real tutorial step. The user does not see it. 
    /// Instead it just blocks the tutorial progress until the player finishes the encounter, after which the player can learn about combat rewards.
    /// </summary>
    public class TutorialStepWaitForCombatEnd: TutorialStep
    {
        /// <summary>
        /// Component which knows about all combatants.
        /// </summary>
        private CombatantsManager combatantsManager;
        /// <summary>
        /// Class which can notify us about game over. If the player looses, the next tutorial step should still not appear until he enters the combat again
        /// </summary>
        private GameStateManager gameOverManager;
        /// <summary>
        /// If the player wipes somehow, we will not end this step until he has restarted combat, so IsTutorialStepOver is blocked until that happens.
        /// </summary>
        private bool stepOverBlockedForGameOver;
        /// <summary>
        /// When a game over happens, the combat is still active.
        /// We need to wait until the player restarts. 
        /// Then it we flag that there was an inactive combat frame since game over.
        /// After that, being in combat means that the player is fighting again.
        /// </summary>
        private bool wasCombatInactiveAfterGameOver;
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        private void Start()
        {
            combatantsManager = FindObjectOfType<CombatantsManager>();
            gameOverManager = FindObjectOfType<GameStateManager>();
            gameOverManager.GameOver += GameOverManager_GameOver;
        }
        /// <summary>
        /// Executed every frame. Detects when the user tries to combat again after game over, see <see cref="wasCombatInactiveAfterGameOver"/>
        /// </summary>
        private void Update()
        {
            if (stepOverBlockedForGameOver)
            {
                wasCombatInactiveAfterGameOver = wasCombatInactiveAfterGameOver || !combatantsManager.IsCombatActive;
                if (wasCombatInactiveAfterGameOver && combatantsManager.IsCombatActive)
                {
                    // Fighting again, if he wins we can end this tutorial step.
                    stepOverBlockedForGameOver = false;
                }    
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        private void OnDestroy()
        {
            gameOverManager.GameOver -= GameOverManager_GameOver;
        }
        /// <summary>
        /// <inheritdoc/>
        /// This step ends when the player wins the first encounter.
        /// </summary>
        public override bool IsTutorialStepOver()
        {
            return !combatantsManager.IsCombatActive && !stepOverBlockedForGameOver;
        }
        /// <summary>
        /// We listen to the game over event in order to block this step until the player retries the combat.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Arguments of the event.</param>
        private void GameOverManager_GameOver(object sender, EventArgs e)
        {
            stepOverBlockedForGameOver = true;
            wasCombatInactiveAfterGameOver = false;
        }
    }
}
