namespace Assets.Scripts.Combat.Skills
{
    class RangedBasicAttack: ProjectileAttack
    {
        RangedBasicAttack()
        {
            BlocksManualMovement = false;
            IsBasicAttack = true;
        }
        protected override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {
            base.Update();
        }
    }
}