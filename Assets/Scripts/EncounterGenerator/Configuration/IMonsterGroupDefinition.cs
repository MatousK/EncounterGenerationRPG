using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
/// <summary>
/// Class implementing this interface can generate a group of monsters when the encounter generator requests it.
/// </summary>
public abstract class MonsterGroupDefinition: ScriptableObject
{
    /// <summary>
    /// Generates a group of monsters based on parameters passed by the generator.
    /// </summary>
    /// <param name="parameters">The parameters for the generation.</param>
    /// <returns>The group of generated monsters. Returns prefabs which should then be instantiated by the generator.</returns>
    public abstract List<GameObject> GenerateMonsterGroup(GenerateMonsterGroupParameters parameters);
}