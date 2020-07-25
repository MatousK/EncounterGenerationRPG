using Assets.Scripts.EncounterGenerator.Model;
using UnityEngine;

namespace Assets.Scripts.EncounterGenerator.Configuration
{
    /// <summary>
    ///  A class that can calculate the target encounter difficulty for the given party strength.
    ///  This was designed when we assumed the difficulty would scale with party strength, essentially that the difficulty would be the strength of enemies.
    ///  Now the only implementation of this class ignores the party parameter.
    /// </summary>
    public abstract class EncounterDifficulty: ScriptableObject
    {
        /// <summary>
        /// Calculates the target encounter difficulty for the specified party strength.
        /// </summary>
        /// <param name="party">The party that is currently fighting.</param>
        /// <returns>The target difficulty of the encounter.</returns>
        public abstract float GetDifficultyForParty(PartyDefinition party);
    }
}