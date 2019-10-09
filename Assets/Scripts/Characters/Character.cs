using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Character : CombatantBase
{
    private MovementController movementController;
    // Start is called before the first frame update
    void Start()
    {
        movementController = GetComponent<MovementController>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public virtual void SkillAttackUsed(Enemy target) { }

    public virtual void AttackUsed(Enemy target)
    {
        GetComponent<AutoAttack>().StartAutoAttack(target);
    }

    public virtual void FriendlySkillUsed(Character target)
    {
        GetComponent<AutoAttack>().StopAutoAttack();
        movementController.MoveToPosition(target.transform.position);
    }

    public virtual void FriendlyClicked(Character target)
    {
        GetComponent<AutoAttack>().StopAutoAttack();
        movementController.MoveToPosition(target.transform.position);
    }

    public virtual void SelfSkillUsed()
    {
        GetComponent<AutoAttack>().StopAutoAttack();
    }

    public virtual void SelfClicked()
    {
        GetComponent<AutoAttack>().StopAutoAttack();
    }

    public virtual void LocationSkillClick(Vector2 position)
    {
        GetComponent<AutoAttack>().StopAutoAttack();
        movementController.MoveToPosition(position);
    }

    public virtual void LocationClick(Vector2 position)
    {
        GetComponent<AutoAttack>().StopAutoAttack();
        movementController.MoveToPosition(position);
    }
}
