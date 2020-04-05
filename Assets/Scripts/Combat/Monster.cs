namespace Assets.Scripts.Combat
{
    public class Monster : CombatantBase
    {
        public MonsterRole Role;
        public MonsterRank Rank;
        protected override void Awake()
        {
            base.Awake();
            CombatantsManager.Enemies.Add(this);
            DamageMaxHitPointsDirectly = true;
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
    }
}
