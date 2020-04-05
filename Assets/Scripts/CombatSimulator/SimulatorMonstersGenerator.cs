using System.Collections.Generic;
using Assets.Scripts.EncounterGenerator.Configuration;
using Assets.Scripts.EncounterGenerator.Model;
using UnityEngine;

namespace Assets.Scripts.CombatSimulator
{
    public class SimulatorMonstersGenerator : MonoBehaviour
    {
        public MonsterGroupDefinition AvailableMonsters;

        public List<GameObject> GenerateMonsters(EncounterDefinition encounter)
        {
            return AvailableMonsters.GenerateMonsterGroup(new GenerateMonsterGroupParameters { RequestedMonsters = encounter });
        }
    }
}