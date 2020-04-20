namespace Assets.Scripts.Combat
{
    public class Monster : CombatantBase
    {
        public MonsterRole Role;
        public MonsterRank Rank;
        protected override void Awake()
        {
            base.Awake();
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

        public override void TakeDamage(int damage, CombatantBase fromCombatant)
        {
            base.TakeDamage(damage, fromCombatant);
            if (IsDown)
            {
                CombatantsManager.Enemies.Remove(this);
            }
        }

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
