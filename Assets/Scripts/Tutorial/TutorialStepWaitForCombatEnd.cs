using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Combat;
using Assets.Scripts.GameFlow;

namespace Assets.Scripts.Tutorial
{
    public class TutorialStepWaitForCombatEnd: TutorialStep
    {
        private CombatantsManager combatantsManager;
        private GameStateManager gameOverManager;
        // If the player wipes somehow, we will not end this step until he has restarted combat, so IsTutorialStepOver is blocked until that happens.
        private bool stepOverBlockedForGameOver;
        private bool wasCombatInactiveAfterGameOver;
        private void Start()
        {
            combatantsManager = FindObjectOfType<CombatantsManager>();
            gameOverManager = FindObjectOfType<GameStateManager>();
            gameOverManager.GameOver += GameOverManager_GameOver;
        }

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

        private void OnDestroy()
        {
            gameOverManager.GameOver -= GameOverManager_GameOver;
        }

        public override bool IsTutorialStepOver()
        {
            return !combatantsManager.IsCombatActive && !stepOverBlockedForGameOver;
        }

        private void GameOverManager_GameOver(object sender, EventArgs e)
        {
            stepOverBlockedForGameOver = true;
            wasCombatInactiveAfterGameOver = false;
        }
    }
}
