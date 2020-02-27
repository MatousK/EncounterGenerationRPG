using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Hero FollowingHero;
    public float ScrollingEdge = 50;
    public float ScrollingSpeed = 0.1f;
    // Used when we are quickly zooming to some specified hero.
    public float QuickScrollingSpeed = 100f;
    bool IsQuickFindHeroInProgress;

    // Update is called once per frame
    void LateUpdate()
    {
        if (IsQuickFindHeroInProgress)
        {
            FollowHeroIfPossible();
            if (Vector2.Distance(transform.position, FollowingHero.transform.position) < 0.1)
            {
                IsQuickFindHeroInProgress = false;
                FollowingHero = null;
            }
            return;
        }
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

    public void QuickFindHero(Hero hero)
    {
        FollowingHero = hero;
        IsQuickFindHeroInProgress = true;
    }

    private void FollowHeroIfPossible()
    {
        if (FollowingHero == null)
        {
            return;
        }
        var speed = IsQuickFindHeroInProgress ? QuickScrollingSpeed : ScrollingSpeed;
        var newPosition = Vector2.MoveTowards(transform.position, FollowingHero.transform.position, speed * Time.unscaledDeltaTime);
        transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
    }
}
