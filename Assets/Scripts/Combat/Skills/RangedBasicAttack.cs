namespace Assets.Scripts.Combat.Skills
{
    /// <summary>
    /// A projectile attack that is marked as a basic attack, i.e. it can be interrupted by movement and other skills.
    /// </summary>
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