using EncounterGenerator.Algorithm;
using EncounterGenerator.Model;
using EncounterGenerator.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class StaticEncounterGenerator: MonoBehaviour
{
    private readonly MonstersManager MonstersManager = new MonstersManager();
    public EncounterDefinition StaticEncounter;
    private readonly RandomWithHistory<MonsterGroupDefinition> MonsterGroupRandom = new RandomWithHistory<MonsterGroupDefinition>();

    public void Start()
    {

    }

    public List<GameObject> GenerateEncounters(EncounterConfiguration configuration, PartyDefinition party)
    {
        if (!configuration.MonsterGroupDefinitions.Any())
        {
            // No monster definitions, so probably no monsters should spawn here.
            return new List<GameObject>();
        }
        var monsterGroupDefinition = MonsterGroupRandom.RandomElementFromSequence(configuration.MonsterGroupDefinitions);
        return MonstersManager.GenerateMonsters(StaticEncounter, monsterGroupDefinition);
    }
}