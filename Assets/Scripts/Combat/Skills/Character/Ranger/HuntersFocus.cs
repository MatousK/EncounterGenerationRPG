using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class HuntersFocus : PersonalSkill
{
    public float DamageMultiplier = 3;
    public float MovementSpeedMultiplier = 0.5f;
    public float AttackSpeedMultiplier = 0.5f;

    CombatantsManager combatantsManager;

    protected override void Awake()
    {
        combatantsManager = FindObjectOfType<CombatantsManager>();
        base.Awake();
    }
    protected override void Update()
    {
        base.Update();
        if (IsActive && !combatantsManager.GetOpponentsFor(selfCombatant, onlyAlive: true).Any())
        {
            // Everyone is dead, noone to attack.
            TryStopSkill();
        }
    }
    protected override void OnPersonalSkillStarted()
    {
        selfCombatant.Attributes.MovementSpeedMultiplier *= AttackSpeedMultiplier;
        selfCombatant.Attributes.AttackSpeedMultiplier *= MovementSpeedMultiplier;
        selfCombatant.Attributes.DealtDamageMultiplier *= DamageMultiplier;
    }

    protected override void OnPersonalSkillStopped()
    {
        selfCombatant.Attributes.MovementSpeedMultiplier /= AttackSpeedMultiplier;
        selfCombatant.Attributes.AttackSpeedMultiplier /= MovementSpeedMultiplier;
        selfCombatant.Attributes.DealtDamageMultiplier /= DamageMultiplier;
    }
}
