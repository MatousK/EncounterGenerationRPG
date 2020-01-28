using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
// Returns a constant difficulty regardless of the party strength
[CreateAssetMenu(menuName = "Encounter generator/Difficulty/Constant", fileName = "Difficulty")]
public class EncounterDifficultyConstant : EncounterDifficulty
{
    public float ConstantDifficulty;
    public override float GetDifficultyForPartyStrength(float partyStrength)
    {
        return ConstantDifficulty;
    }
}