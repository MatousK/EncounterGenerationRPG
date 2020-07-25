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
    /// <summary>
    /// Class which can reconstruct the matrix from the data from the analytics backend.
    /// It will go combat by combat and apply the combat result to the matrix, saving visualizations every step of the way.
    /// This object will destroy itself once done to indicate that it has finished with the reconstruction.
    /// </summary>
    class MatrixReconstructionManager : MonoBehaviour
    {
        /// <summary>
        /// List of all lines belonging to one player this component should go through and apply one by one.
        /// </summary>
        public List<CombatOverLine> CombatOverLines;
        /// <summary>
        /// The folder where we should store the results of the matrix reconstruction.
        /// </summary>
        public string ResultsFolder;
        /// <summary>
        /// The matrix being updated.
        /// </summary>
        public EncounterDifficultyMatrix CurrentMatrix;
        /// <summary>
        /// The version of the experiment, we need to know whether we should emulate the V1 bug.
        /// </summary>
        public int Version;
        /// <summary>
        /// If true, a line is being handled. When Update is called and this is false, we start handling the next line.
        /// This allows us to analyze the data without completely freezing unity.
        /// </summary>
        bool lineHandlingInProgress;
        /// <summary>
        /// The index of the current test being handled.
        /// </summary>
        int currentLineIndex;
        /// <summary>
        /// The component which can log matrix results.
        /// </summary>
        EncounterGeneratorLogger logger;
        /// <summary>
        /// The component which can update the matrix.
        /// </summary>
        EncounterMatrixUpdater matrixUpdater;
        /// <summary>
        /// Called before the first frame update, initializes the matrix reconstruction.
        /// </summary>
        public void Start()
        {
            logger = gameObject.AddComponent<EncounterGeneratorLogger>();
            logger.ResultsDirectory = ResultsFolder;
            var config = Version == 1 ? EncounterGeneratorConfiguration.ConfigurationV1 : EncounterGeneratorConfiguration.ConfigurationV2;
            matrixUpdater = new EncounterMatrixUpdater(CurrentMatrix, config, null);
            matrixUpdater.MatrixChanged += MatrixUpdater_MatrixChanged;
        }
        /// <summary>
        /// Called every frame, starts handling the next line if no line is being handled right now.
        /// And destroys self if all lines are processed.
        /// </summary>
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
        /// <summary>
        /// Call to handle the line with index <see cref="currentLineIndex"/>. 
        /// Increments the index and if the line is supposed to update the matrix, updates the matrix.
        /// </summary>
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
        /// <summary>
        /// Called when the matrix finishes adjusting, visualizes the change and allow to move to the next line.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Arguments of the event.</param>
        private void MatrixUpdater_MatrixChanged(object sender, MatrixChangedEventArgs e)
        {
            logger.LogMatrixChange(e, CurrentMatrix, false);
            lineHandlingInProgress = false;
        }
        /// <summary>
        /// Creates the party definition representing the specific combat line. 
        /// It can be create both the conditions at the start and at the end of combat.
        /// </summary>
        /// <param name="combatLine">Line the party configuration should represent.</param>
        /// <param name="startPartyConfiguration">If true, this will be the party at the start of the combat, otherwise it will be at the end of the combat.</param>
        /// <returns>Party from the specified combat line.</returns>
        private ReconstructionPartyDefinition GetPartyDefinition(CombatOverLine combatLine, bool startPartyConfiguration)
        {
            var hitpoints = startPartyConfiguration ? combatLine.PartyStartHitpoints : combatLine.PartyEndHitpoints;
            return new ReconstructionPartyDefinition(hitpoints, combatLine.PartyAttack);
        }

    }
}
