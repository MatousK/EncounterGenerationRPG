using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public float Speed = 10;
    private Vector3? currentMoveToTarget;
    private CombatantBase selfCombatant;

    private void Start()
    {
        selfCombatant = GetComponent<CombatantBase>();
        if (selfCombatant != null)
        {
            selfCombatant.CombatantDied += MovementComponent_CombatantDied;
        }
    }

    public void MoveToPosition(Vector2 targetPosition)
    {
        if (currentMoveToTarget.HasValue && targetPosition.x == currentMoveToTarget.Value.x && targetPosition.y == currentMoveToTarget.Value.y)
        {
            // Already Moving there.
            return;
        }
        currentMoveToTarget = new Vector3(targetPosition.x, targetPosition.y, transform.position.z);
        // TODO: Get the path we should walk
        var path = new Vector3[] { currentMoveToTarget.Value };
        StartCoroutine(MoveToFollowPath(path));
    }
    public void StopMovement()
    {
        currentMoveToTarget = null;
        GetComponent<Animator>().SetBool("Walking", false);
    }

    private IEnumerator MoveToFollowPath(Vector3[] Path)
    {
        GetComponent<Animator>().SetBool("Walking", true);
        foreach (var pathPoint in Path)
        {
            while (Vector3.Distance(pathPoint, transform.position) > 0.1)
            {
                if (currentMoveToTarget != Path[Path.Length - 1])
                {
                    // Changed target.
                    yield break;
                }
                var speedMultiplier = selfCombatant?.Attributes?.MovementSpeedMultiplier ?? 1;
                GetComponent<Animator>().SetFloat("MovementSpeedMultiplier", speedMultiplier);
                transform.position = Vector3.MoveTowards(transform.position, pathPoint, Speed*Time.deltaTime * speedMultiplier);
                yield return null;
            }
        }
        StopMovement();
    }

    private void OnCollisionEnter2D()
    {
        StopMovement();
    }

    private void MovementComponent_CombatantDied(object sender, System.EventArgs e)
    {
        StopMovement();
    }
}
