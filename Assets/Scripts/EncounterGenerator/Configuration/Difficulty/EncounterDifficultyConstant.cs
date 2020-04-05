using Assets.Scripts.EncounterGenerator.Model;
using UnityEngine;

// Returns a constant difficulty regardless of the party strength
namespace Assets.Scripts.EncounterGenerator.Configuration.Difficulty
{
    [CreateAssetMenu(menuName = "Encounter generator/Difficulty/Constant", fileName = "Difficulty")]
    public class EncounterDifficultyConstant : EncounterDifficulty
    {
        public float ConstantDifficulty;
        public override float GetDifficultyForParty(PartyDefinition party)
        {
            return ConstantDifficulty;
        }
    }
}