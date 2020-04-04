using EncounterGenerator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SimulatorMonstersGenerator : MonoBehaviour
{
    public MonsterGroupDefinition AvailableMonsters;

    public List<GameObject> GenerateMonsters(EncounterDefinition encounter)
    {
        return AvailableMonsters.GenerateMonsterGroup(new GenerateMonsterGroupParameters { RequestedMonsters = encounter });
    }
}