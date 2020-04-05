namespace Assets.Scripts.Combat.Skills
{
    class RangedBasicAttack: ProjectileAttack
    {
        RangedBasicAttack()
        {
            BlocksManualMovement = false;
            IsBasicAttack = true;
        }
        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Update()
        {
            base.Update();
        }
    }
}