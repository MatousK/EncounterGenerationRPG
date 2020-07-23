using System.Linq;

namespace Assets.Scripts.Combat.Skills.Monster.Brute
{
    /// <summary>
    /// A skill which greatly increases the combatant's damage and movement speed. 
    /// A hero with this skill is not selectable (and therefore controllable) while rage is active.
    /// Originally this was a hero skill and might become one again if we implement a barbarian class.
    /// </summary>
    public class Rage : PersonalSkill
    {
        /// <summary>
        /// How much should the damage increase when this skill is active.
        /// </summary>
        public int DamageMultiplier = 4;
        /// <summary>
        /// How much faster should the combatant move when this skill is active.
        /// </summary>
        public float SpeedMultiplier = 4;
        /// <summary>
        /// A class which knows about all combatants that exist.
        /// </summary>
        CombatantsManager combatantsManager;
        /// <summary>
        /// A component of the combatant which controls whether the hero is selected/selectable or not.
        /// </summary>
        SelectableObject selectableComponent;

        public Rage()
        {
            Duration = 5;
            Cooldown = 10;
        }
        /// <summary>
        /// <inheritdoc/>
        /// Stop raging if all opponents are dead so the player can control the character once more.
        /// </summary>
        protected override void Update()
        {
            base.Update();
            if (IsActive && !combatantsManager.GetOpponentsFor(SelfCombatant, onlyAlive:true).Any())
            {
                // Everyone is dead, noone to attack.
                TryStopSkill();
            }
        }
        protected override void Start()
        {
            base.Start();
            combatantsManager = FindObjectOfType<CombatantsManager>();
            selectableComponent = SelfCombatant.GetComponent<SelectableObject>();
        }
        /// <summary>
        /// <inheritdoc/>
        /// Disallows selection and increases the damage and speed multiplier.
        /// </summary>
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
        /// <summary>
        /// <inheritdoc/>
        /// Allows selection and restores the damage and speed multiplier.
        /// </summary>
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
