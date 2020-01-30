using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class EncounterManager: MonoBehaviour
{
    EncounterGenerator EncounterGenerator = new EncounterGenerator();
    CombatantSpawnManager CombatantSpawnManager;

    private void Start()
    {
        var roomsLayout = FindObjectOfType<RoomsLayout>();
        CombatantSpawnManager = FindObjectOfType<CombatantSpawnManager>();
        foreach (var room in roomsLayout.Rooms)
        {
            room.IsExploredChanged += OnRoomExplored;
        }
    }

    private void OnRoomExplored(object sender, RoomExploredEventArgs exploredEventArgs)
    {
        var exploredRoom = sender as RoomInfo;
        var encounter = EncounterGenerator.GenerateEncounters(exploredRoom.RoomEncounter);
        SpawnMonsters(encounter, sender as RoomInfo, exploredEventArgs.IncomingDoors);
    }

    private void SpawnMonsters(List<GameObject> monstersToSpawn, RoomInfo room, Doors incomingDoors)
    {
        foreach (var mosnter in monstersToSpawn)
        {
            CombatantSpawnManager.SpawnCombatant(mosnter, room, incomingDoors, 3);
        }
    }
}