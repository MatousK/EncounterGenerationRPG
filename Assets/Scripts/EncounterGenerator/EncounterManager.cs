using System;
using System.Collections.Generic;
using Assets.Scripts.Combat;
using Assets.Scripts.DungeonGenerator;
using Assets.Scripts.EncounterGenerator.Algorithm;
using Assets.Scripts.EncounterGenerator.Development;
using Assets.Scripts.EncounterGenerator.Model;
using Assets.Scripts.Environment;
using Assets.Scripts.GameFlow;
using UnityEngine;

namespace Assets.Scripts.EncounterGenerator
{
    class EncounterManager: MonoBehaviour
    {
        // TODO: Debug, remove in release builds.
        private StaticEncounterGenerator staticEncounterGenerator;
        private EncounterGenerator encounterGenerator;
        private RoomsLayout roomsLayout;
        private CombatantSpawnManager combatantSpawnManager;
        private CombatantsManager combatantsManager;
        private GameStateManager gameStateManager;
        private EncounterMatrixUpdater matrixUpdater;

        private void Start()
        {
            var difficultyMatrix = FindObjectOfType<DifficultyMatrixProvider>().CurrentDifficultyMatrix;
            matrixUpdater = new EncounterMatrixUpdater(difficultyMatrix);
            encounterGenerator = new EncounterGenerator(difficultyMatrix, matrixUpdater);
            staticEncounterGenerator = GetComponent<StaticEncounterGenerator>();
            roomsLayout = FindObjectOfType<RoomsLayout>();
            combatantSpawnManager = FindObjectOfType<CombatantSpawnManager>();
            combatantsManager = FindObjectOfType<CombatantsManager>();
            gameStateManager = FindObjectOfType<GameStateManager>();
            combatantsManager.CombatOver += CombatantsManager_CombatOver;
            gameStateManager.GameOver += GameStateManager_GameOver;
            foreach (var room in roomsLayout.Rooms)
            {
                room.IsExploredChanged += OnRoomExplored;
            }
        }

        private void GameStateManager_GameOver(object sender, EventArgs e)
        {
            LogCombatResult(true);
        }

        private void CombatantsManager_CombatOver(object sender, EventArgs e)
        {
            LogCombatResult(false);
        }

        private void OnRoomExplored(object sender, RoomExploredEventArgs exploredEventArgs)
        {
            var exploredRoom = (RoomInfo)sender;
            if (!exploredRoom.IsExplored)
            {
                // Room got changed to not explored, does not really interest us.
                return;
            }
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
            foreach (var monster in monstersToSpawn)
            {
                combatantSpawnManager.SpawnCombatant(monster, room, incomingDoors, 5);
            }
        }

        private void LogCombatResult(bool wasGameOver)
        {
            var allHeroes = combatantsManager.PlayerCharacters;
            var partyDefinition = new PartyDefinition { PartyMembers = allHeroes };
            matrixUpdater.CombatOverAdjustMatrix(partyDefinition, wasGameOver);
        }

        private void OnDestroy()
        {
            if (roomsLayout != null)
            {
                foreach (var room in roomsLayout.Rooms)
                {
                    room.IsExploredChanged -= OnRoomExplored;
                }
            }

            if (combatantsManager != null)
            {
                combatantsManager.CombatOver -= CombatantsManager_CombatOver;
            }

            if (gameStateManager != null)
            {
                gameStateManager.GameOver -= GameStateManager_GameOver;
            }
        }
    }
}