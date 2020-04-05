namespace Assets.Scripts.UI
{
    public class CooldownBar : UiProgressBarBase
    {

        public UiBar TotalCooldownIndicator;
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
