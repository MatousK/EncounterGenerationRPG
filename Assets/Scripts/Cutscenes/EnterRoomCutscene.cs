using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Camera;
using Assets.Scripts.Combat;
using Assets.Scripts.DungeonGenerator;
using Assets.Scripts.Environment;
using Assets.Scripts.Movement;
using UnityEngine;

namespace Assets.Scripts.Cutscenes
{
    /// <summary>
    /// Moves the player characters in the entered room.
    /// Works only with up to three players.
    /// </summary>
    class EnterRoomCutscene : Cutscene
    {
        /// <summary>
        /// Doors through which should the characters enter.
        /// </summary>
        public Doors OpenedDoors;
        /// <summary>
        ///  The room which the heroes should enter in this cutscene.
        /// </summary>
        public RoomInfo EnteredRoom;
        /// <summary>
        /// The hero who opened the door.
        /// </summary>
        public Hero DoorOpener;
        CombatantsManager combatantsManager;
        CameraMovement cameraMovement;
        // How many heroes are we waiting for to move to target position. -1 means that movement has not started yet.
        private int movingHeroes;
        // If true, we have already decided position for one back row character.
        // We store this info because first backrow character moves on the cross axis in the positive direction, the other one in the negative direction.
        private bool placeBackRowCharacter;
        private bool HeroesInPosition => movingHeroes == 0;
        private List<SpawnPoint> roomSpawnPoints;

        private void Update()
        {
            EnsureDependenciesLinked();
            if (IsCutsceneActive())
            {
                cameraMovement.FollowingTransform = DoorOpener.transform;
            }
        }
        public override bool IsCutsceneActive()
        {
            return !HeroesInPosition;
        }

        public override void StartCutscene()
        {
            // If too slow, distribute the spawn points between rooms on dungeon load.
            roomSpawnPoints = FindObjectsOfType<SpawnPoint>().Where(spawnPoint =>
                spawnPoint.GetComponent<RoomInfoComponent>().RoomIndex == EnteredRoom.Index).ToList();
            EnsureDependenciesLinked();
            OpenedDoors.CloseInCombat = false;
            foreach (var hero in combatantsManager.GetPlayerCharacters(onlyAlive: true))
            {
                var targetPosition = GetHeroTargetPosition(hero);
                ++movingHeroes;
                print("Moving hero to ." + targetPosition);
                hero.GetComponent<MovementController>().MoveToPosition(targetPosition, ignoreOtherCombatants: true, onMoveToSuccessful: moveSuccessful => {
                    if (!moveSuccessful)
                    {
                        UnityEngine.Debug.LogError("Could not move to target position");
                    }
                    --movingHeroes;
                    });
            }
        }

        public override void EndCutscene()
        {
            EnsureDependenciesLinked();
            OpenedDoors.CloseInCombat = true;
            cameraMovement.FollowingTransform = null;
        }

        private Vector2 GetHeroTargetPosition(Hero hero)
        {
            var heroSpawnPointType = SpawnPoint.GetSpawnPointTypeForCombatant(hero);
            var heroSpawnPoint = roomSpawnPoints.FirstOrDefault(spawnPoint => spawnPoint.Type == heroSpawnPointType);
            if (heroSpawnPoint != null)
            {
                return heroSpawnPoint.transform.position;
            }
            // Bit complicated - basically we want the heroes to get in a formation where the knight is three spaces away from the doors in the direction the party came.
            // The other ones should be only two spaces from the doors, also in the direction from which we came.
            // And as for the other axis (y for horizontal movement and x for vertical movement), we always move in the positive direction if possible.
            // For the second character in the back row this is not possible, as the first one is already there.
            var doorsPosition = OpenedDoors.transform.position;
            float mainAxisMovement;
            float crossAxisMovement;
            if (hero.HeroProfession == HeroProfession.Knight)
            {
                mainAxisMovement = 3f;
                crossAxisMovement = 0.5f;
            }
            else
            {
                mainAxisMovement = 2f;
                crossAxisMovement = placeBackRowCharacter ? -0.5f : 0.5f;
                placeBackRowCharacter = true;
            }
            switch (OpenedDoors.Orientation)
            {
                case DoorOrientation.Horizontal:
                    mainAxisMovement *= DoorOpener.transform.position.x < doorsPosition.x ? 1f : -1f;
                    return new Vector2(doorsPosition.x + mainAxisMovement, doorsPosition.y + crossAxisMovement);
                case DoorOrientation.Vertical:
                    mainAxisMovement *= DoorOpener.transform.position.y < doorsPosition.y ? 1.0f : -1.0f;
                    return new Vector2(doorsPosition.x + crossAxisMovement, doorsPosition.y + mainAxisMovement);
            }
            throw new ArgumentException("Door orientation invalid.");
        }

        private void EnsureDependenciesLinked()
        {
            if (combatantsManager == null || cameraMovement == null)
            {
                combatantsManager = FindObjectOfType<CombatantsManager>();
                cameraMovement = FindObjectOfType<CameraMovement>();
            }
        }
    }
}