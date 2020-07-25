using Assets.Scripts.Combat;
using Assets.Scripts.EncounterGenerator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Experiment.ResultsAnalysis.Model
{
    /// <summary>
    /// <inheritdoc/>
    /// This line has info about the results of a single combat encounter.
    /// </summary>
    public class CombatOverLine: CsvLine
    {
        /// <summary>
        /// Max hit points of every hero at the start of the encounter.
        /// </summary>
        public Dictionary<HeroProfession, float> PartyStartHitpoints;
        /// <summary>
        /// Max hit points of every hero at the end of the encounter.
        /// </summary>
        public Dictionary<HeroProfession, float> PartyEndHitpoints;
        /// <summary>
        /// Attack attributes of each hero.
        /// </summary>
        public Dictionary<HeroProfession, float> PartyAttack;
        /// <summary>
        /// The encounter this line represents.
        /// </summary>
        public EncounterDefinition CombatEncounter;
        /// <summary>
        /// How did the matrix estimate the difficulty of this encounter.
        /// </summary>
        public float ExpectedDifficulty;
        /// <summary>
        /// What was the actual difficulty of the encounter.
        /// </summary>
        public float RealDifficulty;
        /// <summary>
        /// How off was the difficulty estimate.
        /// </summary>
        public float DifficultyError => ExpectedDifficulty - RealDifficulty;
        /// <summary>
        /// If true, the party was wiped in this encounter.
        /// </summary>
        public bool WasGameOver;
        /// <summary>
        /// If true, this was a static encounter, not a generated one.
        /// </summary>
        public bool WasStaticEncounter;
        /// <summary>
        /// If true, this encounter did adjust the difficulty matrix.
        /// </summary>
        public bool WasLogged;
    }
}