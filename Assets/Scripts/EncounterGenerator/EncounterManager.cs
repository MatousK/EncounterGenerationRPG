using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using EncounterGenerator;
using EncounterGenerator.Model;

class EncounterManager: MonoBehaviour
{
    // TODO: Debug, remove in release builds.
    private StaticEncounterGenerator StaticEncounterGenerator;
    readonly EncounterGenerator.EncounterGenerator EncounterGenerator = new EncounterGenerator.EncounterGenerator();
    CombatantSpawnManager CombatantSpawnManager;
    CombatantsManager CombatantsManager;

    private void Start()
    {
        StaticEncounterGenerator = GetComponent<StaticEncounterGenerator>();
        var roomsLayout = FindObjectOfType<RoomsLayout>();
        CombatantSpawnManager = FindObjectOfType<CombatantSpawnManager>();
        CombatantsManager = FindObjectOfType<CombatantsManager>();
        foreach (var room in roomsLayout.Rooms)
        {
            room.IsExploredChanged += OnRoomExplored;
        }
    }

    private void OnRoomExplored(object sender, RoomExploredEventArgs exploredEventArgs)
    {
        var exploredRoom = sender as RoomInfo;
        var allHeroes = CombatantsManager.PlayerCharacters;
        var partyDefinition = new PartyDefinition { PartyMembers = allHeroes };
        List<GameObject> encounter;
        if (StaticEncounterGenerator != null && StaticEncounterGenerator.isActiveAndEnabled)
        {
            encounter = StaticEncounterGenerator.GenerateEncounters(exploredRoom.RoomEncounter, partyDefinition);
        }
        else
        {
            encounter = EncounterGenerator.GenerateEncounters(exploredRoom.RoomEncounter, partyDefinition);
        }
        SpawnMonsters(encounter, sender as RoomInfo, exploredEventArgs.IncomingDoors);
    }

    private void SpawnMonsters(List<GameObject> monstersToSpawn, RoomInfo room, Doors incomingDoors)
    {
        foreach (var mosnter in monstersToSpawn)
        {
            CombatantSpawnManager.SpawnCombatant(mosnter, room, incomingDoors, 5);
        }
    }
}