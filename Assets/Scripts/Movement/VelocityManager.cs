using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityManager : MonoBehaviour
{
    private Vector2 previousFramePosition = Vector2.negativeInfinity;
    private Vector2 velocity = Vector2.zero;
    // Start is called before the first frame update
    void Start()
    {
        previousFramePosition = transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector2 currentPosition = transform.position;
        velocity = currentPosition - previousFramePosition; 
        previousFramePosition = transform.position;
    }

    public Vector2 GetVelocity()
    {
        return velocity;
    }
}
