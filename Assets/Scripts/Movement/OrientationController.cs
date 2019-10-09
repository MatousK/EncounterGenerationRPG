using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientationController : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        var velocity = GetComponent<VelocityManager>().GetVelocity();
        if ((velocity.x > 0 && transform.localScale.x < 0) ||
            (velocity.x < 0 && transform.localScale.x > 0))
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        }
    }
}
