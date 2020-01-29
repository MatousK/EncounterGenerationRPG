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
    // How many heroes are we waiting for to move to target position. -1 means that movement has not started yet.
    private int movingHeroes;
    // If true, we have already decided position for one back row character.
    // We store this info because first backrow character moves on the cross axis in the positive direction, the other one in the negative direction.
    private bool placeBackRowCharacter;
    private bool heroesInPosition
    {
        get
        {
            return movingHeroes == 0;
        }
    }

    private void Awake()
    {
        combatantsManager = FindObjectOfType<CombatantsManager>();
    }
    public override bool IsCutsceneActive()
    {
        return !heroesInPosition;
    }

    public override void StartCutscene()
    {
        OpenedDoors.CloseInCombat = false;
        foreach (var hero in combatantsManager.GetPlayerCharacters(onlyAlive: true))
        {
            var targetPosition = GetHeroTargetPosition(hero);
            ++movingHeroes;
            print("Moving hero to ." + targetPosition);
            hero.GetComponent<MovementController>().MoveToPosition(targetPosition, moveSuccessful =>
            {
                print("Movement finished");
                --movingHeroes;
            });
        }
    }

    public override void EndCutscene()
    {
        OpenedDoors.CloseInCombat = true;
    }

    private Vector2 GetHeroTargetPosition(Hero hero)
    {
        // Bit complicated - basically we want the heroes to get in a formation where the character who opened doors is two spaces away from the doors in the diraction he came.
        // The other ones should be only ony space from the doors, also in the direction from which we came.
        // And as for the other axis (y for horizontal movement and x for verticla movement), we always move in the positive direction if possible.
        // For the second character in the back row this is not possible, as the first one is already there.
        var doorsPosition = OpenedDoors.transform.position;
        float mainAxisMovement;
        float crossAxisMovement;
        if (hero == DoorOpener)
        {
            mainAxisMovement =  2f;
            crossAxisMovement = 0.5f;
        }
        else
        {
            mainAxisMovement = 1f;
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