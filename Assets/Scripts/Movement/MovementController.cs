using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public float Speed = 10;
    private Vector3 currentMoveToTarget = Vector3.negativeInfinity;

    private void Start()
    {
        var movementComponent = GetComponent<CombatantBase>();
        if (movementComponent != null)
        {
            movementComponent.CombatantDied += MovementComponent_CombatantDied;
        }
    }

    public void MoveToPosition(Vector2 targetPosition)
    {
        if (targetPosition.x == this.currentMoveToTarget.x && targetPosition.y == this.currentMoveToTarget.y)
        {
            // Already Moving there.
            return;
        }
        currentMoveToTarget = new Vector3(targetPosition.x, targetPosition.y, transform.position.z);
        // TODO: Get the path we should walk
        var path = new Vector3[] { currentMoveToTarget };
        StartCoroutine(MoveToFollowPath(path));
    }
    public void StopMovement()
    {
        currentMoveToTarget = Vector3.negativeInfinity;
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
                transform.position = Vector3.MoveTowards(transform.position, pathPoint, Speed*Time.deltaTime);
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
