using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public float Speed = 10;
    private Vector3 currentMoveToTarget = Vector3.negativeInfinity;
    private Collider collider;

    private void Start()
    {
        collider = GetComponent<Collider>();
    }

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

    private void OnCollisionEnter2D()
    {
        currentMoveToTarget = Vector3.negativeInfinity;
        GetComponent<Animator>().SetBool("Walking", false);
    }
}
