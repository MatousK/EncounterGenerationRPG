using System.Linq;

namespace Assets.Scripts.Combat.Skills.Character.Ranger
{
    /// <summary>
    /// A stance which increases attack speed while reducing damage and movement speed.
    /// </summary>
    class RapidStance : PersonalSkill
    {
        /// <summary>
        /// How much should the damage decrease while this skill is active.
        /// </summary>
        public float DamageMultiplier = 0.75f;
        /// <summary>
        /// How much should the movement speed decrease while this skill is active.
        /// </summary>
        public float MovementSpeedMultiplier = 0.5f;
        /// <summary>
        /// How much should the attack speed decrease while this skill is active.
        /// </summary>
        public float AttackSpeedMultiplier = 2f;
        /// <summary>
        /// The class which knows about all combatants in the game.
        /// </summary>
        CombatantsManager combatantsManager;

        protected override void Start()
        {
            combatantsManager = FindObjectOfType<CombatantsManager>();
            base.Start();
        }
        /// <summary>
        /// <inheritdoc/>
        /// Stops the skill when all enemies are dead.
        /// </summary>
        protected override void Update()
        {
            base.Update();
            if (IsActive && !combatantsManager.GetOpponentsFor(SelfCombatant, onlyAlive: true).Any())
            {
                // Everyone is dead, noone to attack.
                TryStopSkill();
            }
        }
        /// <summary>
        /// <inheritdoc/> Changes the movement speed, attack speed and damage multiplier.
        /// </summary>
        protected override void OnPersonalSkillStarted()
        {
            SelfCombatant.Attributes.MovementSpeedMultiplier *=  MovementSpeedMultiplier;
            SelfCombatant.Attributes.AttackSpeedMultiplier *= AttackSpeedMultiplier;
            SelfCombatant.Attributes.DealtDamageMultiplier *= DamageMultiplier;
        }
        /// <summary>
        /// <inheritdoc/> Restores the movement speed, attack speed and damage multiplier.
        /// </summary>
        protected override void OnPersonalSkillStopped()
        {
            SelfCombatant.Attributes.MovementSpeedMultiplier /= MovementSpeedMultiplier;
            SelfCombatant.Attributes.AttackSpeedMultiplier /= AttackSpeedMultiplier;
            SelfCombatant.Attributes.DealtDamageMultiplier /= DamageMultiplier;
        }
    }
}
