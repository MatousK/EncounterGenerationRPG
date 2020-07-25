using System.Collections.Generic;
using Assets.Scripts.EncounterGenerator.Model;
using UnityEngine;

namespace Assets.Scripts.EncounterGenerator.Configuration
{
    /// <summary>
    /// Parameters passed to the <see cref="MonsterGroup"/>. Specify what monsters the monster group should generate.
    /// </summary>
    public struct GenerateMonsterGroupParameters
    {
        /// <summary>
        /// The monsters we should generate - how many monsters of each monster type should be returned.
        /// </summary>
        public EncounterDefinition RequestedMonsters;
        /// <summary>
        /// Which monsters should be more likely to appear. 0 is neutral, positive is they should appear more likely, negative is that they should not appear if possible.
        /// </summary>
        public Dictionary<GameObject, float> MonsterPriorities;
    }
}