namespace Assets.Scripts.Combat.Skills
{
    class BasicAttack : Attack
    {
        BasicAttack()
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
