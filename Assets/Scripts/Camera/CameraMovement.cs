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
        if (mouseX < ScrollingEdge && mouseX > 0)
        {
            transform.Translate(new Vector3(-ScrollingSpeed * Time.deltaTime, 0, 0));
        }
        else if (mouseX > Screen.width - ScrollingEdge && mouseX <= Screen.width)
        {
            transform.Translate(new Vector3(ScrollingSpeed * Time.deltaTime, 0, 0));
        }
        if (mouseY < ScrollingEdge && mouseY >= 0)
        {
            transform.Translate(new Vector3(0,-ScrollingSpeed * Time.deltaTime, 0));
        }
        else if (mouseY > Screen.height - ScrollingEdge && mouseY <= Screen.height)
        {
            transform.Translate(new Vector3(0,ScrollingSpeed * Time.deltaTime, 0));
        }
    }
}
