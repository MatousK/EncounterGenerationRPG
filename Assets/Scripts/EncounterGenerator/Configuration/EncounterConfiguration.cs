using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.EncounterGenerator.Configuration
{
    /// <summary>
    /// Specifies which encounters can appear in a specific room.
    /// </summary>
    [CreateAssetMenu(menuName = "Encounter generator/Encounter Definition", fileName = "EncounterDefinition")]
    public class EncounterConfiguration : ScriptableObject
    {
        /// <summary>
        /// Specifies how difficult the encounter should be.
        /// </summary>
        public EncounterDifficulty EncounterDifficulty;
        /// <summary>
        /// Specifies which monsters should spawn in this room.
        /// </summary>
        public List<MonsterGroupDefinition> MonsterGroupDefinitions;
    }
}
