using System.Collections.Generic;
using Assets.Scripts.EncounterGenerator.Configuration;
using Assets.Scripts.EncounterGenerator.Model;
using UnityEngine;

namespace Assets.Scripts.CombatSimulator
{
    /// <summary>
    /// This component can get a list of monsters to spawn for a specified encounter.
    /// </summary>
    public class SimulatorMonstersGenerator : MonoBehaviour
    {
        /// <summary>
        /// List of all monsters that can be spawned. Should be a list of all monsters in the game.
        /// </summary>
        public MonsterGroupDefinition AvailableMonsters;
        /// <summary>
        /// Creates a list of monsters that fit the specified encounter.
        /// </summary>
        /// <param name="encounter">An encounter specifying the monsters that can be spawned.</param>
        /// <returns>The list of monsters that should be spawned. The returned values are templates and will need to be instantiated by <see cref="Object.Instantiate(Object)"/></returns>
        public List<GameObject> GenerateMonsters(EncounterDefinition encounter)
        {
            return AvailableMonsters.GenerateMonsterGroup(new GenerateMonsterGroupParameters { RequestedMonsters = encounter });
        }
    }
}