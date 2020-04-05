using System.Collections.Generic;
using Assets.Scripts.Combat;
using Assets.Scripts.DungeonGenerator;
using Assets.Scripts.EncounterGenerator.Development;
using Assets.Scripts.EncounterGenerator.Model;
using Assets.Scripts.Environment;
using UnityEngine;

namespace Assets.Scripts.EncounterGenerator
{
    class EncounterManager: MonoBehaviour
    {
        // TODO: Debug, remove in release builds.
        private StaticEncounterGenerator staticEncounterGenerator;
        readonly Assets.Scripts.EncounterGenerator.EncounterGenerator encounterGenerator = new Assets.Scripts.EncounterGenerator.EncounterGenerator();
        CombatantSpawnManager combatantSpawnManager;
        CombatantsManager combatantsManager;

        private void Start()
        {
            staticEncounterGenerator = GetComponent<StaticEncounterGenerator>();
            var roomsLayout = FindObjectOfType<RoomsLayout>();
            combatantSpawnManager = FindObjectOfType<CombatantSpawnManager>();
            combatantsManager = FindObjectOfType<CombatantsManager>();
            foreach (var room in roomsLayout.Rooms)
            {
                room.IsExploredChanged += OnRoomExplored;
            }
        }

        private void OnRoomExplored(object sender, RoomExploredEventArgs exploredEventArgs)
        {
            var exploredRoom = sender as RoomInfo;
            var allHeroes = combatantsManager.PlayerCharacters;
            var partyDefinition = new PartyDefinition { PartyMembers = allHeroes };
            List<GameObject> encounter;
            if (staticEncounterGenerator != null && staticEncounterGenerator.isActiveAndEnabled)
            {
                encounter = staticEncounterGenerator.GenerateEncounters(exploredRoom.RoomEncounter);
            }
            else
            {
                encounter = encounterGenerator.GenerateEncounters(exploredRoom.RoomEncounter, partyDefinition);
            }
            SpawnMonsters(encounter, sender as RoomInfo, exploredEventArgs.IncomingDoors);
        }

        private void SpawnMonsters(List<GameObject> monstersToSpawn, RoomInfo room, Doors incomingDoors)
        {
            foreach (var mosnter in monstersToSpawn)
            {
                combatantSpawnManager.SpawnCombatant(mosnter, room, incomingDoors, 5);
            }
        }
    }
}