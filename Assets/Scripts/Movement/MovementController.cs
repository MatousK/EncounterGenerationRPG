using System.Collections.Generic;
using Assets.Scripts.Combat;
using Assets.Scripts.Movement.Pathfinding;
using UnityEngine;

namespace Assets.Scripts.Movement
{
    /// <summary>
    /// Component which can move a character around.
    /// Uses paths provided by <see cref="Pathfinder"/> class.
    /// </summary>
    public class MovementController : MonoBehaviour
    {
        /// <summary>
        /// Delegate for method executed upon movement finish.
        /// </summary>
        /// <param name="successful">If true, the character successfully reaches the destination.</param>
        public delegate void MovementCompletion(bool successful);
        /// <summary>
        /// The speed of the combatant, how many squares per second can he pass.
        /// </summary>
        public float Speed = 10;
        /// <summary>
        /// If true, the current movement ignores other combatants.
        /// </summary>
        private bool ignoringCombatants = false;
        /// <summary>
        /// The target of the current move in progress.
        /// </summary>
        private Vector2Int? currentMoveToTarget;
        /// <summary>
        /// The delegate to call when this movement finishes.
        /// </summary>
        private MovementCompletion currentMoveToCompletion;
        /// <summary>
        /// The current path we are walking on.
        /// </summary>
        private Queue<Vector2Int> currentMoveToPath;
        /// <summary>
        /// World space coordinates of the space we are currently moving to.
        /// </summary>
        private Vector3? nextSquareWorldSpace;
        /// <summary>
        /// The combatant attached to the same object this component is attached to.
        /// </summary>
        private CombatantBase selfCombatant;
        /// <summary>
        /// The component used for calculating paths to targets.
        /// </summary>
        private Pathfinder pathfinder;
        /// <summary>
        /// The component which provides pathfinding maps.
        /// </summary>
        PathfindingMapController pathfindingMapController;
        /// <summary>
        /// The grid on which the game is playing.
        /// </summary>
        public Grid MapGrid;
        /// <summary>
        /// If true, we are currently in the middle of a move.
        /// </summary>
        public bool IsMoving => currentMoveToTarget != null;
        /// <summary>
        /// Called before the first frame. Finds references to dependencies and subscribes to events.
        /// </summary>
        private void Start()
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
        /// <summary>
        /// Executed after all other Update methods.
        /// We use it so we can execute move to commands immediately.
        /// Moves to the target location if there is one set.
        /// </summary>
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
            var speedMultiplier = selfCombatant.Attributes.MovementSpeedMultiplier;
            GetComponent<Animator>().SetFloat("MovementSpeedMultiplier", speedMultiplier);
            transform.position = Vector3.MoveTowards(transform.position, nextSquareWorldSpace.Value, Speed * Time.deltaTime * speedMultiplier);
        }
        /// <summary>
        /// Called when we reached the next space on the path. Sets the next target.
        /// If the next square on the path is occupied, recalculate the path.
        /// </summary>
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
            var worldSpacePosition = nextSquareWorldSpace ?? transform.position;
            var grid3DPosition = MapGrid.WorldToCell(worldSpacePosition);
            return new Vector2Int(grid3DPosition.x, grid3DPosition.y);
        }

        /// <summary>
        /// Moves to the specified location. 
        /// </summary>
        /// <param name="targetPosition">The position where the hero is moving.</param>
        /// <param name="onMoveToSuccessful">To be called if we successfully navigate to the target position.</param>
        /// <param name="ignoreOtherCombatants">If true, pathfinding will ignore all other combatants.</param>
        /// <param name="animate"> If true, the combatant will do a walking animation.</param>
        public void MoveToPosition(Vector2 targetPosition, bool ignoreOtherCombatants = false, MovementCompletion onMoveToSuccessful = null, bool animate = true)
        {
            currentMoveToCompletion?.Invoke(false);
            ignoringCombatants = ignoreOtherCombatants;
            currentMoveToCompletion = onMoveToSuccessful;
            if (animate)
            {
                GetComponent<Animator>().SetBool("Walking", true);
            }
            CalculateAndSavePathToTargetWorldSpace(targetPosition);
        }
        /// <summary>
        /// Call to force the agent to stop.
        /// </summary>
        /// <param name="movementSuccessful">If true, we have moved to the target space successfully. Otherwise false.</param>
        public void StopMovement(bool movementSuccessful = false)
        {
            // We cache the completion.
            // Why? Because each agent only has one completion method.
            // There is no gurantee (it even happened before) that currentMoveToCompletion won't start or end another move with its own completion.
            // Which means to be sure we must clear the completion before calling the completion of this move.
            var cachedCompletion = currentMoveToCompletion;
            currentMoveToTarget = null;
            currentMoveToCompletion = null;
            GetComponent<Animator>().SetBool("Walking", false);
            cachedCompletion?.Invoke(movementSuccessful);
        }
        /// <summary>
        /// Calculates the path to the world space position where we want to move. .
        /// Saves it if found. Update will then try to move there
        /// </summary>
        /// <param name="targetPositionWorldSpace">The target world space position.</param>
        private void CalculateAndSavePathToTargetWorldSpace(Vector2 targetPositionWorldSpace)
        {
            var targetGridSpace = MapGrid.WorldToCell(targetPositionWorldSpace);
            CalculateAndSavePathToTargetGridSpace(new Vector2Int(targetGridSpace.x, targetGridSpace.y));
        }
        /// <summary>
        /// Calculates the path to the grid space position where we want to move.
        /// Saves it if found. Update will then try to move there
        /// </summary>
        /// <param name="targetPositionGridSpace">The target grid space position.</param>
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
        /// <summary>
        /// Stop all movement when the combatant dies attached to the same object as this component dies.
        /// Dead people don't really move around much.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Arguments of the event.</param>
        private void MovementComponent_CombatantDied(object sender, System.EventArgs e)
        {
            StopMovement(movementSuccessful: false);
        }
        /// <summary>
        /// Unsubscribe from events on destroyed.
        /// </summary>
        private void OnDestroy()
        {
            if (selfCombatant != null)
            {
                selfCombatant.CombatantDied -= MovementComponent_CombatantDied;
            }
        }
    }
}
