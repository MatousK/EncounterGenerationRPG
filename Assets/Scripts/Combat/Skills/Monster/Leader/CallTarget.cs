using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// Orders all other monsters to attack the target.
/// </summary>
public class CallTarget : TargetedGestureSkill
{
    CombatantsManager combatantsManager;
    /// <summary>
    /// Determines how long will the target call last.
    /// </summary>
    public float Duration;

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
        foreach (var alliedMonster in combatantsManager.GetEnemies(onlyAlive: true))
        {
            if (alliedMonster == selfCombatant)
            {
                // The leader has free will.
                continue;
            }
            var conditionManager = alliedMonster.GetComponent<ConditionManager>();
            var addedCondition = conditionManager.AddCondition<ForcedTargetCondition>();
            addedCondition.ForcedTarget = Target;
            addedCondition.RemainingDuration = Duration;
            addedCondition.TargetForcedBy = selfCombatant;
        }
    }
}