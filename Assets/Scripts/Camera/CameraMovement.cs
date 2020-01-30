using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Hero FollowingHero;
    public float ScrollingEdge = 50;
    public float ScrollingSpeed = 0.1f;

    // Update is called once per frame
    void LateUpdate()
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
            FollowingHero = null;
            transform.Translate(new Vector3(-ScrollingSpeed * Time.unscaledDeltaTime, 0, 0));
        }
        else if (mouseX > Screen.width - ScrollingEdge)
        {
            FollowingHero = null;
            transform.Translate(new Vector3(ScrollingSpeed * Time.unscaledDeltaTime, 0, 0));
        }
        if (mouseY < ScrollingEdge)
        {
            FollowingHero = null;
            transform.Translate(new Vector3(0, -ScrollingSpeed * Time.unscaledDeltaTime, 0));
        }
        else if (mouseY > Screen.height - ScrollingEdge)
        {
            FollowingHero = null;
            transform.Translate(new Vector3(0, ScrollingSpeed * Time.unscaledDeltaTime, 0));
        }
        FollowHeroIfPossible();
    }

    private void FollowHeroIfPossible()
    {
        if (FollowingHero == null)
        {
            return;
        }
        var newPosition = Vector2.MoveTowards(transform.position, FollowingHero.transform.position, ScrollingSpeed * Time.unscaledDeltaTime);
        transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
    }
}
