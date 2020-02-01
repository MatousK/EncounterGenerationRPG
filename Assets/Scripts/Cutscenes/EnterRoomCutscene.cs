using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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
    private bool HeroesInPosition
    {
        get
        {
            return movingHeroes == 0;
        }
    }

    private void Awake()
    {
        combatantsManager = FindObjectOfType<CombatantsManager>();
        cameraMovement = FindObjectOfType<CameraMovement>();
    }

    private void Update()
    {
        if (IsCutsceneActive())
        {
            cameraMovement.FollowingHero = DoorOpener;
        }
    }
    public override bool IsCutsceneActive()
    {
        return !HeroesInPosition;
    }

    public override void StartCutscene()
    {
        OpenedDoors.CloseInCombat = false;
        foreach (var hero in combatantsManager.GetPlayerCharacters(onlyAlive: true))
        {
            var targetPosition = GetHeroTargetPosition(hero);
            ++movingHeroes;
            print("Moving hero to ." + targetPosition);
            hero.GetComponent<MovementController>().MoveToPosition(targetPosition, ignoreOtherCombatants:true, onMoveToSuccessful: moveSuccessful => OnMoveToFinished(hero, targetPosition));
        }
    }

    public override void EndCutscene()
    {
        OpenedDoors.CloseInCombat = true;
        cameraMovement.FollowingHero = null;
    }

    private void OnMoveToFinished(Hero hero, Vector2 originalTarget)
    {
        // Movement finished but it might have been unsuccessful, something might have been in the way - check whether the hero is on the correct side of the door.
        var doorPosition = OpenedDoors.transform.position;
        var doorMainCoordinate = OpenedDoors.Orientation == DoorOrientation.Vertical ? doorPosition.y : doorPosition.x;
        var targetMainCoordinate = OpenedDoors.Orientation == DoorOrientation.Vertical ? originalTarget.y : originalTarget.x;
        var heroMainCoordinate = OpenedDoors.Orientation == DoorOrientation.Vertical ? hero.transform.position.y : hero.transform.position.x;
        bool isOnCorrectSide = (doorMainCoordinate < targetMainCoordinate) == (doorMainCoordinate < heroMainCoordinate);
        print("Movement finished");
        --movingHeroes;
        Debug.Assert(isOnCorrectSide, "We ended up on the wrong side of the door, which should not happen.");
    }

    private Vector2 GetHeroTargetPosition(Hero hero)
    {
        // Bit complicated - basically we want the heroes to get in a formation where the character who opened doors is three spaces away from the doors in the diraction he came.
        // The other ones should be only two spaces from the doors, also in the direction from which we came.
        // And as for the other axis (y for horizontal movement and x for vertical movement), we always move in the positive direction if possible.
        // For the second character in the back row this is not possible, as the first one is already there.
        var doorsPosition = OpenedDoors.transform.position;
        float mainAxisMovement;
        float crossAxisMovement;
        if (hero == DoorOpener)
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
                mainAxisMovement *= hero.transform.position.x < doorsPosition.x ? 1f : -1f;
                return new Vector2(doorsPosition.x + mainAxisMovement, doorsPosition.y + crossAxisMovement);
            case DoorOrientation.Vertical:
                mainAxisMovement *= hero.transform.position.y < doorsPosition.y ? 1.0f : -1.0f;
                return new Vector2(doorsPosition.x + crossAxisMovement, doorsPosition.y + mainAxisMovement);
        }
        throw new ArgumentException("Door orientation invalid.");
    }
}