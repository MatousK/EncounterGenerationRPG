using EncounterGenerator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
/// <summary>
///  A class that can calculate the target encounter difficulty for the given party strength.
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