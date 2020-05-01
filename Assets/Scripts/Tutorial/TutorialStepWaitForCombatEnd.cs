using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Combat;

namespace Assets.Scripts.Tutorial
{
    public class TutorialStepWaitForCombatEnd: TutorialStep
    {
        private CombatantsManager combatantsManager;
        private void Start()
        {
            combatantsManager = FindObjectOfType<CombatantsManager>();
        }
        public override bool IsTutorialStepOver()
        {
            return !combatantsManager.IsCombatActive;
        }
    }
}
