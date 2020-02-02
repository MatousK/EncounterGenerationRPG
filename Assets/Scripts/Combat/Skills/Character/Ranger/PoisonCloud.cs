using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class PoisonCloud : TargetedGestureSkill
{
    public float PoisonCloudRange = 3;
    public float PoisonDuration = 10;
    public GameObject PoisonCloudEffect;
    private CombatantsManager combatantsManager;

    protected override void Awake()
    {
        base.Awake();
        combatantsManager = FindObjectOfType<CombatantsManager>();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void ApplySkillEffects(object sender, EventArgs e)
    {
        Instantiate(PoisonCloudEffect, Target.transform, false);
        foreach (var enemy in combatantsManager.GetEnemies(onlyAlive:true))
        {
            if (Vector2.Distance(enemy.transform.position, Target.transform.position) < PoisonCloudRange)
            {
                var poisonCondition = enemy.GetComponent<ConditionManager>().AddCondition<PoisonedCondition>();
                poisonCondition.RemainingDuration = PoisonDuration;
            }
        }
    }
}