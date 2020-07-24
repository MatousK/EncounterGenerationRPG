using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.EncounterGenerator.Configuration;
using Assets.Scripts.EncounterGenerator.Model;
using UnityEngine;

namespace Assets.Scripts.EncounterGenerator.Algorithm
{
    /// <summary>
    /// This class can generate monsters for a specified encounter definition.
    /// It tries to vary the monsters if possible.
    /// </summary>
    public class MonstersManager
    {
        /// <summary>
        /// Map specifying the likelihood for each monster to be generated. All start at 1, whenever a monster is selected its value is reset to 1. All other monsters' weights are incremented.
        /// </summary>
        private readonly Dictionary<GameObject, float> monsterPriorities = new Dictionary<GameObject, float>();
        /// <summary>
        /// Generates encoutners for the specified encounter definition.
        /// </summary>
        /// <param name="encounterDefinition">Encounter definition for which we want monsters.</param>
        /// <param name="monsterGroupDefinition">The object which specifies which monsters should actually be generated.</param>
        /// <returns></returns>
        public List<GameObject> GenerateMonsters(EncounterDefinition encounterDefinition, MonsterGroupDefinition monsterGroupDefinition)
        {
            var parameters = new GenerateMonsterGroupParameters
            {
                RequestedMonsters = encounterDefinition,
                MonsterPriorities = monsterPriorities
            };
            var generatedMonsters = monsterGroupDefinition.GenerateMonsterGroup(parameters);
            // Raise the chance of generating monsters that were not generated right now and reset chances of those already generated.
            foreach (var existingMonster in monsterPriorities.Keys.ToList())
            {
                monsterPriorities[existingMonster]++;
            }
            foreach (var monster in generatedMonsters)
            {
                monsterPriorities[monster] = 1;
            }
            return generatedMonsters;
        }
    }
}
