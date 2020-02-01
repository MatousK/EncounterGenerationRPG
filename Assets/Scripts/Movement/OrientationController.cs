using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientationController : MonoBehaviour
{
    // If true, the sprite is mirrored, so our orientations should also be mirrored
    public bool IsSpriteMirrored = false;
    public Vector2? LookAtTarget = null;
    // Update is called once per frame
    void Update()
    {
        var transformMultiplier = IsSpriteMirrored ? -1 : 1;
        Vector2 orientationVector;
        if (LookAtTarget == null)
        {
            orientationVector = GetComponent<VelocityManager>().GetVelocity();
        } else
        {
            Vector2 currentPosition = transform.position;
            orientationVector = LookAtTarget.Value - currentPosition;
        }
        if ((orientationVector.x > 0 && transform.localScale.x * transformMultiplier < 0) ||
            (orientationVector.x < 0 && transform.localScale.x * transformMultiplier > 0))
        {
            transform.localScale = new Vector3(transform.localScale.x * -1 * transformMultiplier, transform.localScale.y, transform.localScale.z);
        }
    }
}
