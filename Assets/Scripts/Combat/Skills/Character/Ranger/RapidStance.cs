using System.Linq;

namespace Assets.Scripts.Combat.Skills.Character.Ranger
{
    class RapidStance : PersonalSkill
    {
        public float DamageMultiplier = 0.75f;
        public float MovementSpeedMultiplier = 0.5f;
        public float AttackSpeedMultiplier = 2f;

        CombatantsManager combatantsManager;

        protected override void Awake()
        {
            combatantsManager = FindObjectOfType<CombatantsManager>();
            base.Awake();
        }
        protected override void Update()
        {
            base.Update();
            if (IsActive && !combatantsManager.GetOpponentsFor(SelfCombatant, onlyAlive: true).Any())
            {
                // Everyone is dead, noone to attack.
                TryStopSkill();
            }
        }
        protected override void OnPersonalSkillStarted()
        {
            SelfCombatant.Attributes.MovementSpeedMultiplier *=  MovementSpeedMultiplier;
            SelfCombatant.Attributes.AttackSpeedMultiplier *= AttackSpeedMultiplier;
            SelfCombatant.Attributes.DealtDamageMultiplier *= DamageMultiplier;
        }

        protected override void OnPersonalSkillStopped()
        {
            SelfCombatant.Attributes.MovementSpeedMultiplier /= MovementSpeedMultiplier;
            SelfCombatant.Attributes.AttackSpeedMultiplier /= AttackSpeedMultiplier;
            SelfCombatant.Attributes.DealtDamageMultiplier /= DamageMultiplier;
        }
    }
}
