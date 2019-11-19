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
    CombatantBase combatant;

    protected override void Start()
    {
        combatant = GetComponent<CombatantBase>();
        combatantsManager = FindObjectOfType<CombatantsManager>();
        base.Start();
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
        combatant.Attributes.MovementSpeedMultiplier *= AttackSpeedMultiplier;
        combatant.Attributes.AttackSpeedMultiplier *= MovementSpeedMultiplier;
        combatant.Attributes.DealtDamageMultiplier *= DamageMultiplier;
    }

    protected override void OnPersonalSkillStopped()
    {
        combatant.Attributes.MovementSpeedMultiplier /= AttackSpeedMultiplier;
        combatant.Attributes.AttackSpeedMultiplier /= MovementSpeedMultiplier;
        combatant.Attributes.DealtDamageMultiplier /= DamageMultiplier;
    }
}
