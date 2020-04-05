namespace Assets.Scripts.Combat.Skills
{
    class BasicAttack : Attack
    {
        BasicAttack()
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
