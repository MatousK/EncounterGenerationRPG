using Assets.Scripts.EncounterGenerator.Model;
using UnityEngine;

// Returns a constant difficulty regardless of the party strength
namespace Assets.Scripts.EncounterGenerator.Configuration.Difficulty
{
    /// <summary>
    /// <inheritdoc/>
    /// This implementation returns a constant number regardless of the party strength.
    /// </summary>
    [CreateAssetMenu(menuName = "Encounter generator/Difficulty/Constant", fileName = "Difficulty")]
    public class EncounterDifficultyConstant : EncounterDifficulty
    {
        /// <summary>
        /// The difficulty that should be always returned.
        /// </summary>
        public float ConstantDifficulty;
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="party">The party who will fight in the encounter.</param>
        /// <returns>The difficulty of the encounter that should be generated.</returns>
        public override float GetDifficultyForParty(PartyDefinition party)
        {
            return ConstantDifficulty;
        }
    }
}