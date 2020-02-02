using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Taunt : PersonalSkill
{
    public float ReceivedDamageModifier = 0.5f;
    public float TauntDuration;
    private CombatantsManager combatantsManager;
    public Taunt()
    {
        SkillAnimationName = "";

    }
    protected override void OnPersonalSkillStarted()
    {
        selfCombatant.GetComponentInChildren<TauntEffect>().StartEffect();
        selfCombatant.Attributes.ReceivedDamageMultiplier *= ReceivedDamageModifier;
        foreach (var enemy in combatantsManager.GetEnemies(onlyAlive: true))
        {
            var tauntCondition = enemy.GetComponent<ConditionManager>().AddCondition<ForcedTargetCondition>();
            tauntCondition.RemainingDuration = TauntDuration;
            tauntCondition.ForcedTarget = selfCombatant;
            tauntCondition.TargetForcedBy = selfCombatant;
        }
    }

    protected override void OnPersonalSkillStopped()
    {
        selfCombatant.Attributes.ReceivedDamageMultiplier /= ReceivedDamageModifier;
    }

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        combatantsManager = FindObjectOfType<CombatantsManager>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
}
