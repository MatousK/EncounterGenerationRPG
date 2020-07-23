namespace Assets.Scripts.Combat.Skills
{
    /// <summary>
    /// An attack skill that is marked as a basic attack, i.e. it does not block movements and other attacks.
    /// </summary>
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
