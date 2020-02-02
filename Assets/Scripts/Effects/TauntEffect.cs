using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TauntEffect : MonoBehaviour
{
    /// <summary>
    /// Minimum size of the circle at the beginning of the animation.
    /// </summary>
    public float minRadius = 0;
    /// <summary>
    /// Maximum radius of the circle at the end of the animation.
    /// </summary>
    public float maxRadius = 100;
    /// <summary>
    /// How long should the animation be.
    /// </summary>
    public float animationLength = 1;
    private float radiusChangePerSecond;
    private Circle animatedCircle;

    private void Start()
    {
        animatedCircle = GetComponent<Circle>();
        radiusChangePerSecond = maxRadius / animationLength;
    }

    public void StartEffect()
    {
        animatedCircle.IsVisible = true;
        animatedCircle.Radius = minRadius;
        StartCoroutine(TauntAnimation());
    }

    private IEnumerator TauntAnimation()
    {
        while (animatedCircle.Radius < maxRadius)
        {
            animatedCircle.Radius += Time.deltaTime * radiusChangePerSecond;
            yield return null;
        }
        animatedCircle.IsVisible = false;
    }
}
