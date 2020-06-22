using Assets.Scripts.EncounterGenerator.Algorithm;
using Assets.Scripts.EncounterGenerator.Configuration;
using Assets.Scripts.EncounterGenerator.Model;
using Assets.Scripts.EncounterGenerator.Utils;
using Assets.Scripts.Experiment.ResultsAnalysis.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Experiment.ResultsAnalysis
{
    class MatrixReconstructionManager : MonoBehaviour
    {
        public List<CombatOverLine> CombatOverLines;
        public string ResultsFolder;
        public EncounterDifficultyMatrix CurrentMatrix;
        public int Version;
        bool lineHandlingInProgress;
        int currentLineIndex;
        EncounterGeneratorLogger logger;
        EncounterMatrixUpdater matrixUpdater;
        public void Start()
        {
            logger =gameObject.AddComponent<EncounterGeneratorLogger>();
            logger.ResultsDirectory = ResultsFolder;
            var config = Version == 1 ? EncounterGeneratorConfiguration.ConfigurationV1 : EncounterGeneratorConfiguration.ConfigurationV2;
            matrixUpdater = new EncounterMatrixUpdater(CurrentMatrix, config, null);
            matrixUpdater.MatrixChanged += MatrixUpdater_MatrixChanged;
        }

        public void Update()
        {
            if (lineHandlingInProgress)
            {
                return;
            }
            if (currentLineIndex >= CombatOverLines.Count)
            {
                Destroy(this);
                return;
            }
            HandleCurrentCombatResult();
        }

        private void HandleCurrentCombatResult()
        {
            var combatLine = CombatOverLines[currentLineIndex++];
            if (combatLine.WasLogged)
            {
                lineHandlingInProgress = true;
                matrixUpdater.StoreCombatStartConditions(GetPartyDefinition(combatLine, true), combatLine.CombatEncounter, combatLine.ExpectedDifficulty);
                matrixUpdater.CombatOverAdjustMatrix(GetPartyDefinition(combatLine, false), combatLine.WasGameOver);
            }
        }    

        private void MatrixUpdater_MatrixChanged(object sender, MatrixChangedEventArgs e)
        {
            logger.LogMatrixChange(e, CurrentMatrix, false);
            lineHandlingInProgress = false;
        }

        private ReconstructionPartyDefinition GetPartyDefinition(CombatOverLine combatLine, bool startPartyConfiguration)
        {
            var hitpoints = startPartyConfiguration ? combatLine.PartyStartHitpoints : combatLine.PartyEndHitpoints;
            return new ReconstructionPartyDefinition(hitpoints, combatLine.PartyAttack);
        }

    }
}
