using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
/// <summary>
/// Parameters used for generating a group of monsters.
/// </summary>
public struct GenerateMonsterGroupParameters
{
    /// <summary>
    /// How monsters with which roles should be generated.
    /// </summary>
    public List<KeyValuePair<MonsterTypeDefinition, int>> RequestedMonsters;
    /// <summary>
    /// Which monsters should be more likely to appear. 0 is neutral, positive is they should appear more likely, negative is that they should not appear if possible.
    /// </summary>
    public Dictionary<GameObject, float> MonsterPriorities;
    /// <summary>
    /// How difficult should the target encounter be.
    /// </summary>
    public float TargetEncounterDifficulty;
}

public struct MonsterTypeDefinition
{
    public MonsterTypeDefinition(MonsterRank Rank, MonsterRole Role)
    {
        this.Rank = Rank;
        this.Role = Role;
    }
    public MonsterRank Rank;
    public MonsterRole Role;
}