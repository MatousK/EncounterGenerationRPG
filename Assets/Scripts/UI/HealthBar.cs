namespace Assets.Scripts.UI
{
    /// <summary>
    /// Health bar showing how many hit points a combatant has. Total Max HP, current Max HP and current HP,
    /// </summary>
    public class HealthBar : UiProgressBarBase
    {
        /// <summary>
        /// The line showing the total max hit points. Should always be full scale.
        /// </summary>
        public UiBar TotalMaxHitPointsIndicator;
        /// <summary>
        /// Shows how many percent of Max HP does the character have left.
        /// </summary>
        public UiBar CurrentMaxHitPointsIndicator;
        /// <summary>
        /// Shows how many percent of HP does the character have left.
        /// </summary>
        public UiBar CurrentHitPointsIndicator;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
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
        /// <summary>
        /// Updates the health bars to match the character's health.
        /// </summary>
        protected override void UpdateIndicators()
        {
            CurrentMaxHitPointsIndicator.Percentage = (float)RepresentedCombatant.MaxHitpoints / RepresentedCombatant.TotalMaxHitpoints;
            CurrentHitPointsIndicator.Percentage = (float)RepresentedCombatant.HitPoints / RepresentedCombatant.TotalMaxHitpoints;
        }
    }
}
