namespace Assets.Scripts.Combat.Skills.Monster.Lurker
{
    /// <summary>
    /// An attack that damages the enemy multiple times in swift sequence.
    /// </summary>
    public class ThousandBlades : Attack
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThousandBlades"/> class.
        /// </summary>
        public ThousandBlades()
        {
            Cooldown = 5;
            Repetitions = 10;
            Speed = 5;
            DamagePerHit = 1;
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void Start()
        {
            base.Start();
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
