namespace Assets.Scripts.Combat
{
    /// <summary>
    /// Represents a single enemy for the player to fight.
    /// Contains only the info about the monster, all behavior must be added to other components.
    /// </summary>
    public class Monster : CombatantBase
    {
        /// <summary>
        /// Role of the enemy, how it should behave in combat and what skills it has.
        /// </summary>
        public MonsterRole Role;
        /// <summary>
        /// The rank of the monster, how powerful it is.
        /// </summary>
        public MonsterRank Rank;
        protected override void Awake()
        {
            base.Awake();
            // Monsters only survive a single encounter, so there is no reason for them to also loose first HP and then Max HP like players.
            DamageMaxHitPointsDirectly = true;
        }

        protected override void Start()
        {
            base.Start();
            CombatantsManager.Enemies.Add(this);
        }
        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
        }
        /// <summary>
        /// <inheritdoc/>. Removes itself automatically from CombatantsManager if defeated.
        /// </summary>
        /// <param name="damage">How much damager should be taken.</param>
        /// <param name="fromCombatant">Combatant responsible for this damage.</param>
        public override void TakeDamage(int damage, CombatantBase fromCombatant)
        {
            base.TakeDamage(damage, fromCombatant);
            if (IsDown)
            {
                CombatantsManager.Enemies.Remove(this);
            }
        }
        /// <summary>
        /// <inheritdoc/>. Will also heal Max HP, as for monsters, max HP is HP.
        /// </summary>
        /// <param name="healAmount">How healed should the monster be.</param>
        /// <param name="fromCombatant">Combatant responsible for the damage.</param>
        /// <param name="withDefaultAnimation">Animation done as part of the heal animation.</param>
        public override void HealDamage(float healAmount, CombatantBase fromCombatant, bool withDefaultAnimation = true)
        {
            var healAmountModified = (int)(healAmount * Attributes.ReceivedHealingMultiplier);
            MaxHitpoints += healAmountModified;
            if (MaxHitpoints > TotalMaxHitpoints)
            {
                MaxHitpoints = TotalMaxHitpoints;
            }
            base.HealDamage(healAmount, fromCombatant, withDefaultAnimation);
        }
    }
}
