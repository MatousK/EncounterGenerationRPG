using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public float Speed = 10;
    private Vector3? currentMoveToTarget;
    private List<Vector3> currentMoveToPath;
    private CombatantBase selfCombatant;
    private Pathfinder pathfinder;
    public bool IsMoving
    {
        get
        {
            return currentMoveToTarget != null;
        }
    }

    private void Start()
    {
        pathfinder = FindObjectOfType<Pathfinder>();
        selfCombatant = GetComponent<CombatantBase>();
        if (selfCombatant != null)
        {
            selfCombatant.CombatantDied += MovementComponent_CombatantDied;
        }
    }

    private void LateUpdate()
    {
        if (currentMoveToTarget == null)
        {
            return;
        }
        if (currentMoveToPath == null || currentMoveToPath.Count == 0)
        {
            StopMovement();
            return;
        }
        var pathPoint = currentMoveToPath[0];
        if (Vector3.Distance(pathPoint, transform.position) < 0.05)
        {
            currentMoveToPath.RemoveAt(0);
            if (currentMoveToPath.Count == 0)
            {
                StopMovement();
                return;
            }
            pathPoint = currentMoveToPath[0];
        }
        var speedMultiplier = selfCombatant?.Attributes?.MovementSpeedMultiplier ?? 1;
        GetComponent<Animator>().SetFloat("MovementSpeedMultiplier", speedMultiplier);
        transform.position = Vector3.MoveTowards(transform.position, pathPoint, Speed * Time.deltaTime * speedMultiplier);
    }

    public void MoveToPosition(Vector2 targetPosition)
    {
        if (currentMoveToTarget.HasValue && targetPosition.x == currentMoveToTarget.Value.x && targetPosition.y == currentMoveToTarget.Value.y)
        {
            // Already Moving there.
            return;
        }
        CalculateAndSavePathToTarget(targetPosition);
        GetComponent<Animator>().SetBool("Walking", true);
    }
    public void StopMovement()
    {
        currentMoveToTarget = null;
        GetComponent<Animator>().SetBool("Walking", false);
    }
    private void CalculateAndSavePathToTarget(Vector2 targetPosition)
    {
        var path = pathfinder.FindPath(transform.position, targetPosition, selfCombatant);
        if (path == null || path.Count == 0)
        {
            // No path to target found or we're already there.
            return;
        }
        List<Vector3> path3D = path.ConvertAll((vector2) => new Vector3(vector2.x, vector2.y, transform.position.z));
        currentMoveToTarget = path3D[path3D.Count - 1];
        currentMoveToPath = path3D;
    }

    private void OnCollisionEnter2D(Collision2D colision)
    {
        // Colliding. Recalculate path to the target.
        if (currentMoveToTarget != null)
        {
            CalculateAndSavePathToTarget(currentMoveToTarget.Value);
        }
    }

    private void MovementComponent_CombatantDied(object sender, System.EventArgs e)
    {
        StopMovement();
    }
}
