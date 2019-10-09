using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public float Speed = 10;
    private Vector3 currentMoveToTarget = Vector3.negativeInfinity;
    public void MoveToPosition(Vector3 TargetPosition)
    {
        currentMoveToTarget = TargetPosition;
        // TODO: Get the path we should walk
        var path = new Vector3[] { TargetPosition };
        StartCoroutine(MoveToFollowPath(path));
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
        GetComponent<Animator>().SetBool("Walking", false);
    }
}
