using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientationController : MonoBehaviour
{
    public GameObject LookAtTarget;
    // Update is called once per frame
    void Update()
    {
        Vector2 orientationVector;
        if (LookAtTarget == null)
        {
            orientationVector = GetComponent<VelocityManager>().GetVelocity();
        } else
        {
            orientationVector = LookAtTarget.transform.position - transform.position;
        }
        if ((orientationVector.x > 0 && transform.localScale.x < 0) ||
            (orientationVector.x < 0 && transform.localScale.x > 0))
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        }
    }
}
