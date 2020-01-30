using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public delegate void MovementCompletion(bool successful);
    public float Speed = 10;
    private bool ignoringCombatants = false;
    private Vector2Int? currentMoveToTarget;
    private MovementCompletion currentMoveToCompletion;
    private Queue<Vector2Int> currentMoveToPath;
    private Vector3? nextSquareWorldSpace;
    private CombatantBase selfCombatant;
    private Pathfinder pathfinder;
    PathfindingMapController pathfindingMapController;
    public Grid MapGrid;
    public bool IsMoving
    {
        get
        {
            return currentMoveToTarget != null;
        }
    }

    private void Awake()
    { 
        pathfindingMapController = FindObjectOfType<PathfindingMapController>();
        MapGrid = FindObjectOfType<Grid>();
        pathfinder = FindObjectOfType<Pathfinder>();
        selfCombatant = GetComponent<CombatantBase>();
        if (selfCombatant != null)
        {
            selfCombatant.CombatantDied += MovementComponent_CombatantDied;
        }
    }

    private void LateUpdate()
    {
        if (!IsMoving)
        {
            // Not moving anywhere.
            return;
        }
        if (nextSquareWorldSpace != null && Vector3.Distance(nextSquareWorldSpace.Value, transform.position) < 0.05)
        {
            // Reached next square we wanted to go to. Set the next one.
            SetNextMoveToSquare();
        }
        if (nextSquareWorldSpace == null)
        {
            // Reached target.
            StopMovement(movementSuccessful: true);
            return;
        }
        var speedMultiplier = selfCombatant?.Attributes?.MovementSpeedMultiplier ?? 1;
        GetComponent<Animator>().SetFloat("MovementSpeedMultiplier", speedMultiplier);
        transform.position = Vector3.MoveTowards(transform.position, nextSquareWorldSpace.Value, Speed * Time.deltaTime * speedMultiplier);
    }

    private void SetNextMoveToSquare()
    {
        if (currentMoveToPath == null || currentMoveToPath.Count == 0)
        {
            nextSquareWorldSpace = null;
            return;
        }
        var nextSpace = currentMoveToPath.Dequeue();
        if (!ignoringCombatants && !pathfindingMapController.GetPassabilityMapForCombatant(selfCombatant).GetSquareIsPassable(nextSpace))
        {
            //Something is in our way, recalculate the path.
            CalculateAndSavePathToTargetGridSpace(currentMoveToTarget.Value);
            return;
        }
        nextSquareWorldSpace = MapGrid.GetCellCenterWorld(new Vector3Int(nextSpace.x, nextSpace.y, 0));
        nextSquareWorldSpace = new Vector3(nextSquareWorldSpace.Value.x, nextSquareWorldSpace.Value.y, transform.position.z);
    }
    /// <summary>
    /// Get position where this combatant wants to be, either the current position or position of a neighbouring square it is moving to right now.
    /// Or null if the character is dead.
    /// </summary>
    public Vector2Int? GetReservedGridPosition()
    {
        if (GetComponent<CombatantBase>().IsDown)
        {
            // Dead characters should not bother the living.
            return null;
        }
        var worldSpacePosition = nextSquareWorldSpace != null ? nextSquareWorldSpace.Value : transform.position;
        var grid3DPosition = MapGrid.WorldToCell(worldSpacePosition);
        return new Vector2Int(grid3DPosition.x, grid3DPosition.y);
    }

    /// <summary>
    /// Moves the underlying hero to a specified location. 
    /// </summary>
    /// <param name="targetPosition">The position where the hero is moving.</param>
    /// <param name="onMoveToSuccessful">To be called if we successfuly navigate to the target postiion.</param>
    public void MoveToPosition(Vector2 targetPosition, bool ignoreOtherCombatants = false, MovementCompletion onMoveToSuccessful = null)
    {
        ignoringCombatants = ignoreOtherCombatants;
        currentMoveToCompletion = onMoveToSuccessful;
        GetComponent<Animator>().SetBool("Walking", true);
        CalculateAndSavePathToTargetWorldSpace(targetPosition);
    }
    public void StopMovement(bool movementSuccessful = false)
    {
        // We cache the completion because we can only call it once we stop movement, as completion might stop another movement which would stop.
        var cachedCompletion = currentMoveToCompletion;
        currentMoveToTarget = null;
        currentMoveToCompletion = null;
        GetComponent<Animator>().SetBool("Walking", false);
        cachedCompletion?.Invoke(movementSuccessful);
    }
    private void CalculateAndSavePathToTargetWorldSpace(Vector2 targetPositionWorldSpace)
    {
        var targetGridSpace = MapGrid.WorldToCell(targetPositionWorldSpace);
        CalculateAndSavePathToTargetGridSpace(new Vector2Int(targetGridSpace.x, targetGridSpace.y));
    }

    private void CalculateAndSavePathToTargetGridSpace(Vector2Int targetPositionGridSpace)
    {
        currentMoveToTarget = targetPositionGridSpace;
        var path = pathfinder.FindPath(transform.position, targetPositionGridSpace, selfCombatant, ignoringCombatants);
        if (path == null || path.Count == 0)
        {
            // No path to target found or we're already there.
            StopMovement(movementSuccessful: true);
            return;
        }
        currentMoveToPath = new Queue<Vector2Int>(path);
        SetNextMoveToSquare();
    }
    private void MovementComponent_CombatantDied(object sender, System.EventArgs e)
    {
        StopMovement(movementSuccessful: false);
    }
}
