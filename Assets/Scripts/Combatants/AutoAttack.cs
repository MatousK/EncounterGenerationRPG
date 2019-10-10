using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoAttack : MonoBehaviour
{
    public int Range = 1;
    public int DamagePerHit = 1;
    public float Speed = 1;
    public bool IsAutoAttacking
    {
        get
        {
            return target != null;
        }
    }
    private CombatantBase target;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null || target.IsDown)
        {
            GetComponent<Animator>().SetBool("Attacking", false);
            GetComponent<OrientationController>().LookAtTarget = null;
            target = null;
            return;
        }
        if (GetComponent<Collider2D>().Distance(target.GetComponent<Collider2D>()).distance > Range)
        {
            GetComponent<Animator>().SetBool("Attacking", false);
            GetComponent<OrientationController>().LookAtTarget = null;
            GetComponent<MovementController>().MoveToPosition(target.transform.position);
            return;
        }
        GetComponent<OrientationController>().LookAtTarget = target.gameObject;
        GetComponent<Animator>().SetBool("Attacking", true);
    }

    public void StartAutoAttack(CombatantBase target)
    {
        this.target = target;
    }
    public void StopAutoAttack()
    {
        target = null;
    }

    public void AttackHit()
    {
        target.DealDamage(DamagePerHit);
    }
}
