using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float ScrollingEdge = 50;
    public float ScrollingSpeed = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var mouseY = Input.mousePosition.y;
        var mouseX = Input.mousePosition.x;

        if (mouseX < 0 || mouseY < 0 || mouseX > Screen.width || mouseY > Screen.height)
        {
            // Mouse out of bounds;
            return;
        }

        if (mouseX < ScrollingEdge)
        {
            transform.Translate(new Vector3(-ScrollingSpeed * Time.deltaTime, 0, 0));
        }
        else if (mouseX > Screen.width - ScrollingEdge)
        {
            transform.Translate(new Vector3(ScrollingSpeed * Time.deltaTime, 0, 0));
        }
        if (mouseY < ScrollingEdge)
        {
            transform.Translate(new Vector3(0,-ScrollingSpeed * Time.deltaTime, 0));
        }
        else if (mouseY > Screen.height - ScrollingEdge)
        {
            transform.Translate(new Vector3(0,ScrollingSpeed * Time.deltaTime, 0));
        }
    }
}
