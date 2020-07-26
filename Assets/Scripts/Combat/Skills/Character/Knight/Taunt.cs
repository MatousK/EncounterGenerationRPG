using Assets.Scripts.Combat.Conditions;
using Assets.Scripts.Effects;

namespace Assets.Scripts.Combat.Skills.Character.Knight
{
    /// <summary>
    /// Forces all enemies to attack the caster. Also reduces incoming damage for the taunt duration.
    /// </summary>
    public class Taunt : PersonalSkill
    {
        /// <summary>
        /// How much should the damage to the hero decrease.
        /// </summary>
        public float ReceivedDamageModifier = 0.5f;
        /// <summary>
        /// How long should the taunt work.
        /// </summary>
        public float TauntDuration;
        /// <summary>
        /// An object which knows about the positions of all combatants on the map.
        /// </summary>
        private CombatantsManager combatantsManager;
        /// <summary>
        /// Initializes a new instance of the <see cref="Taunt"/> class.
        /// </summary>
        public Taunt()
        {
            SkillAnimationName = "";

        }
        /// <summary>
        /// <inheritdoc/> Will go through all enemies and force them to attack the knight, <see cref="ForcedTargetCondition"/>.
        /// Also reduces the <see cref="CombatantAttributes.ReceivedDamageMultiplier"/> of the caster.
        /// Will also show the taunt special effect to indicate to the player that the taunt was activated.
        /// </summary>
        protected override void OnPersonalSkillStarted()
        {
            SelfCombatant.GetComponentInChildren<TauntEffect>().StartEffect();
            SelfCombatant.Attributes.ReceivedDamageMultiplier *= ReceivedDamageModifier;
            foreach (var enemy in combatantsManager.GetEnemies(onlyAlive: true))
            {
                var tauntCondition = enemy.GetComponent<ConditionManager>().AddCondition<ForcedTargetCondition>();
                tauntCondition.RemainingDuration = TauntDuration;
                tauntCondition.ForcedTarget = SelfCombatant;
                tauntCondition.TargetForcedBy = SelfCombatant;
            }
        }
        /// <summary>
        /// <inheritdoc/> Restores the <see cref="CombatantAttributes.ReceivedDamageMultiplier"/> of the caster.
        /// </summary>
        protected override void OnPersonalSkillStopped()
        {
            SelfCombatant.Attributes.ReceivedDamageMultiplier /= ReceivedDamageModifier;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void Start()
        {
            base.Start();
            combatantsManager = FindObjectOfType<CombatantsManager>();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void Update()
        {
            base.Update();
        }
    }
}
