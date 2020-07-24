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
    /// A cutscene that plays when the player opens the door.
    /// Moves the player characters in the entered room.
    /// Works only with up to three players.
    /// If there are spawn points present in the room, the characters will go there.
    /// If there are not spawn points for the characters, the characters will create a formation in front of the door
    /// through which they came. Knight in the front, the other two characters in the back.
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
        /// <summary>
        /// The object which knows about all the combatants in the game.
        /// </summary>
        CombatantsManager combatantsManager;
        /// <summary>
        /// The object that can move the camera around.
        /// </summary>
        CameraMovement cameraMovement;
        /// <summary>
        /// How many heroes are still moving to the target position. -1 means that movement has not started yet.
        /// 0 means that all heroes are in position.
        /// </summary>
        private int movingHeroes;
        /// <summary>
        /// If true, we have already decided position for one back row character.
        /// We store this info because first backrow character moves on the cross axis in the positive direction, the other one in the negative direction.
        /// </summary>
        private bool didPlaceBackRowCharacter;
        /// <summary>
        /// If true, all heroes have successfully moved to the target positions.
        /// </summary>
        private bool heroesInPosition => movingHeroes == 0;
        /// <summary>
        /// All spawn points present in the room we should enter.
        /// </summary>
        private List<SpawnPoint> roomSpawnPoints;

        private void Update()
        {
            EnsureDependenciesLinked();
            if (IsCutsceneActive())
            {
                cameraMovement.FollowingTransform = DoorOpener.transform;
            }
        }
        /// <summary>
        /// <inheritdoc/>
        /// This cutscene ends once all heroes move to the target position.
        /// </summary>
        /// <returns>True if a cutscene is currently playing.</returns>
        public override bool IsCutsceneActive()
        {
            return !heroesInPosition;
        }
        /// <summary>
        /// <inheritdoc/>
        /// Orders all heroes to move to the specified positions in the room.
        /// </summary>
        public override void StartCutscene()
        {
            // If too slow, distribute the spawn points between rooms on dungeon load.
            roomSpawnPoints = FindObjectsOfType<SpawnPoint>().Where(spawnPoint =>
                spawnPoint.GetComponent<RoomInfoComponent>().RoomIndex == EnteredRoom.Index).ToList();
            EnsureDependenciesLinked();
            // We want these doors to remain opened until our characters successfully move into the room.
            OpenedDoors.CloseInCombat = false;
            foreach (var hero in combatantsManager.GetPlayerCharacters(onlyAlive: true))
            {
                var targetPosition = GetHeroTargetPosition(hero);
                ++movingHeroes;
                print("Moving hero to ." + targetPosition);
                hero.GetComponent<MovementController>().MoveToPosition(targetPosition, ignoreOtherCombatants: true, onMoveToSuccessful: moveSuccessful => {
                    if (!moveSuccessful)
                    {
                        // We count the hero as in position, because we have no idea why this happened and how to actually get the character in position.
                        // TODO: Maybe we could teleport the hero to the correct position when this happens.
                        UnityEngine.Debug.LogError("Could not move to target position");
                    }
                    --movingHeroes;
                    });
            }
        }
        /// <summary>
        /// <inheritdoc/>
        /// Closes the doors through which we came and stops the camera movement.
        /// </summary>
        public override void EndCutscene()
        {
            EnsureDependenciesLinked();
            OpenedDoors.CloseInCombat = true;
            cameraMovement.FollowingTransform = null;
        }
        /// <summary>
        /// Get the location to which this hero should move during this cutscene.
        /// </summary>
        /// <param name="hero">The hero who should move to the target location.</param>
        /// <returns>The location to which the hero should move.</returns>
        private Vector2 GetHeroTargetPosition(Hero hero)
        {
            var heroSpawnPointType = SpawnPoint.GetSpawnPointTypeForCombatant(hero);
            var heroSpawnPoint = roomSpawnPoints.FirstOrDefault(spawnPoint => spawnPoint.Type == heroSpawnPointType);
            if (heroSpawnPoint != null)
            {
                // Found a spawn point which trumps everything else.
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
                crossAxisMovement = didPlaceBackRowCharacter ? -0.5f : 0.5f;
                didPlaceBackRowCharacter = true;
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
        /// <summary>
        /// Makes sure that we have the references to all required dependencies.
        /// </summary>
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