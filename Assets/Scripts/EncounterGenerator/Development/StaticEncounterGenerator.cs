using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.EncounterGenerator.Algorithm;
using Assets.Scripts.EncounterGenerator.Configuration;
using Assets.Scripts.EncounterGenerator.Model;
using Assets.Scripts.EncounterGenerator.Utils;
using UnityEngine;

namespace Assets.Scripts.EncounterGenerator.Development
{
    /// <summary>
    /// Class used in development to generate a list of specific encounters in a room.
    /// This was used when we could not yet specify the static list of monsters in the level graph.
    /// However, this still could be useful in the future when testing new monster definitions, as we might want to test <see cref="MonsterGroupDefinition"/> children against specific encounters.
    /// But right now it would not work, as no other class calls <see cref="GenerateEncounters(EncounterConfiguration)"/>
    /// </summary>
    public class StaticEncounterGenerator: MonoBehaviour
    {
        /// <summary>
        /// The encounter which should spawn here.
        /// </summary>
        public EncounterDefinition StaticEncounter;
        /// <summary>
        /// The objects which keeps the history of which monsters were spawned. Can create monsters for an encounter and monster group and update the history.
        /// </summary>
        private readonly MonstersManager monstersManager = new MonstersManager();
        /// <summary>
        /// This object is for selecting a random monster group from a list. Uses weights to make it more likely to select monster groups which were not selected in some time.
        /// </summary>
        private readonly RandomWithHistory<MonsterGroupDefinition> monsterGroupRandom = new RandomWithHistory<MonsterGroupDefinition>();
        /// <summary>
        /// Creates the encounters for the <see cref="StaticEncounter"/> and a monster group definition from <see cref="monsterGroupRandom"/>.
        /// </summary>
        /// <param name="configuration">The generic configuration for the encounter generation.</param>
        /// <returns>The list of monsters to return.</returns>
        public List<GameObject> GenerateEncounters(EncounterConfiguration configuration)
        {
            if (!configuration.MonsterGroupDefinitions.Any())
            {
                // No monster definitions, so probably no monsters should spawn here.
                return new List<GameObject>();
            }
            var monsterGroupDefinition = monsterGroupRandom.RandomElementFromSequence(configuration.MonsterGroupDefinitions);
            return monstersManager.GenerateMonsters(StaticEncounter, monsterGroupDefinition);
        }
    }
}