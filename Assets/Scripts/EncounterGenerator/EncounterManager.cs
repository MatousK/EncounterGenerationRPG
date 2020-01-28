using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class EncounterManager: MonoBehaviour
{
    EncounterGenerator EncounterGenerator = new EncounterGenerator();
    RoomsLayout RoomsLayout;
    CombatantSpawnManager CombatantSpawnManager;
    public List<EncounterConfiguration> EncounterConfigurations;

    private void Start()
    {
        RoomsLayout = FindObjectOfType<RoomsLayout>();
        CombatantSpawnManager = FindObjectOfType<CombatantSpawnManager>();
        foreach (var room in RoomsLayout.Rooms)
        {
            room.IsExploredChanged += OnRoomExplored;
        }
    }

    private void OnRoomExplored(object sender, bool isExplored)
    {
        if (!isExplored)
        {
            // Room is still not explored;
            return;
        }
        int roomIndex = RoomsLayout.Rooms.IndexOf(sender as RoomInfo);
        if (roomIndex < 0 || roomIndex >= EncounterConfigurations.Count)
        {
            throw new ArgumentOutOfRangeException("sender");
        }
        var encounterConfiguration = EncounterConfigurations[roomIndex];
        var encounter = EncounterGenerator.GenerateEncounters(encounterConfiguration);
        print(encounter.Count);
        SpawnMonsters(encounter, sender as RoomInfo);
    }

    private void SpawnMonsters(List<GameObject> monstersToSpawn, RoomInfo room)
    {
        foreach (var mosnter in monstersToSpawn)
        {
            CombatantSpawnManager.SpawnCombatant(mosnter, room);
        }
    }
}