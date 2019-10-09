using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatantBase : MonoBehaviour
{
    public int HitPoints;
    public int MaxHitpoints;
    public bool IsDown
    {
        get
        {
            return HitPoints <= 0;
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (IsDown)
        {
            GetComponent<Animator>().SetBool("Dead", true);
        }
    }
}
