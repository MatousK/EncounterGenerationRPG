namespace Assets.Scripts.UI
{
    /// <summary>
    /// The bar showing the cooldown indicator above a character.
    /// </summary>
    public class CooldownBar : UiProgressBarBase
    {
        /// <summary>
        /// The indicator that stays the same, the black back showing the total cooldown time.
        /// </summary>
        public UiBar TotalCooldownIndicator;
        /// <summary>
        /// The indicator which shows how much progress was made. When it matches <see cref="TotalCooldownIndicator"/>, cooldown is over.
        /// </summary>
        public UiBar CooldownProgressIndicator;

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
        }
        /// <summary>
        /// Finds the cooldown values of the combatant and uses them to update the scale of the indicators.
        /// Also toggles their visibility based on whether there is a cooldown active or not.
        /// </summary>
        protected override void UpdateIndicators()
        {
            // We do not show the indicator if we the cooldown is not active.
            if (!RepresentedCombatant.LastSkillRemainingCooldown.HasValue || !RepresentedCombatant.LastSkillCooldown.HasValue || RepresentedCombatant.LastSkillRemainingCooldown < 0)
            {
                TotalCooldownIndicator.gameObject.SetActive(false);
                CooldownProgressIndicator.gameObject.SetActive(false);
                return;
            }
            TotalCooldownIndicator.gameObject.SetActive(true);
            CooldownProgressIndicator.gameObject.SetActive(true);
            // We do not show time remaining, instead we show how much time has already passed.
            var progressPercentage = RepresentedCombatant.LastSkillRemainingCooldown.Value / RepresentedCombatant.LastSkillCooldown.Value;
            CooldownProgressIndicator.Percentage = 1f - progressPercentage;
        }
    }
}
