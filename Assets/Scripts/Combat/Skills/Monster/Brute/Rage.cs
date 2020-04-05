using System.Linq;

namespace Assets.Scripts.Combat.Skills.Monster.Brute
{
    public class Rage : PersonalSkill
    {
        public int DamageMultiplier = 4;
        public float SpeedMultiplier = 4;

        CombatantsManager combatantsManager;
        SelectableObject selectableComponent;

        public Rage()
        {
            Duration = 5;
            Cooldown = 10;
        }

        protected override void Update()
        {
            base.Update();
            if (IsActive && !combatantsManager.GetOpponentsFor(SelfCombatant, onlyAlive:true).Any())
            {
                // Everyone is dead, noone to attack.
                TryStopSkill();
            }
        }
        protected override void Awake()
        {
            base.Awake();
            combatantsManager = FindObjectOfType<CombatantsManager>();
            selectableComponent = SelfCombatant.GetComponent<SelectableObject>();
        }
        protected override void OnPersonalSkillStarted()
        {
            if (selectableComponent != null)
            {
                selectableComponent.IsSelected = false;
                selectableComponent.IsSelectionEnabled = false;
            }

            SelfCombatant.Attributes.MovementSpeedMultiplier *= SpeedMultiplier;
            SelfCombatant.Attributes.AttackSpeedMultiplier *= SpeedMultiplier;
            SelfCombatant.Attributes.DealtDamageMultiplier *= DamageMultiplier;
        }

        protected override void OnPersonalSkillStopped()
        {
            if (selectableComponent != null)
            {
                selectableComponent.enabled = true;
                selectableComponent.IsSelectionEnabled = true;
            }

            SelfCombatant.Attributes.MovementSpeedMultiplier /= SpeedMultiplier;
            SelfCombatant.Attributes.AttackSpeedMultiplier /= SpeedMultiplier;
            SelfCombatant.Attributes.DealtDamageMultiplier /= DamageMultiplier;
        }
    }
}
