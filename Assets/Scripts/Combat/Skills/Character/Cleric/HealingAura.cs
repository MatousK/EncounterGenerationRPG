using Assets.Scripts.Tutorial;
using UnityEngine;

namespace Assets.Scripts.Combat.Skills.Character.Cleric
{
    /// <summary>
    /// A skill which starts healing allies of the combatant. It will create a pulse every now and then which will heal allies currently nearby.
    /// </summary>
    class HealingAura : PersonalSkill
    {
        public HealingAura()
        {
            SkillAnimationName = "";
        }
        /// <summary>
        /// This is a hotfix for a balance issue regarding the cleric boss enemy.
        /// Normally he was healing 40 HP per second, which is more than the combined attack
        /// of cleric and knight, so he was pretty much unkillable. This allows us to lower
        /// self healing for some monsters if necessary.
        /// </summary>
        public float SelfHealingModifier = 1;
        /// <summary>
        /// How close must the ally be for the aura to work.
        /// </summary>
        public float AuraRange = 0;
        /// <summary>
        ///  How many times per second should the heal pulse fire.
        /// </summary>
        public float HealPulseFrequency = 0;
        /// <summary>
        /// How much healing should the aura do per pulse, in percents.
        /// </summary>
        public float HealPulsePercentage = 10;
        /// <summary>
        /// The aura object which should be activated while this skill is active.
        /// This is a visual indicator for the player that the aura is active.
        /// </summary>
        public GameObject AuraInstance = null;
        /// <summary>
        /// Returns how much time is between heal pulses.
        /// </summary>
        private float PulseTime => 1 / HealPulseFrequency;
        /// <summary>
        /// The time left before another pulse should fire.
        /// </summary>
        private float timeToNextPulse = float.PositiveInfinity;
        /// <summary>
        /// The object which knows about all the combatants in the game.
        /// </summary>
        private CombatantsManager combatantsManager;
        // Start is called before the first frame update
        protected override void Start()
        {
            combatantsManager = FindObjectOfType<CombatantsManager>();
            base.Start();
        }

        /// <summary>
        /// <inheritdoc/>. Updates <see cref="timeToNextPulse"/> by <see cref="Time.deltaTime"/> and if necessary starts a healing pulse, <see cref="HealPulse"/>
        /// </summary>
        protected override void Update()
        {
            timeToNextPulse -= Time.deltaTime;
            if (timeToNextPulse <= 0)
            {
                HealPulse();
            }
            base.Update();
        }
        /// <summary>
        /// Heals all allies in distance <see cref="AuraRange"/> by <see cref="HealPulsePercentage"/> health.
        /// Will also reset <see cref="timeToNextPulse"/>.
        /// </summary>
        private void HealPulse()
        {
            timeToNextPulse = PulseTime;
            foreach (var ally in combatantsManager.GetAlliesFor(SelfCombatant))
            {
                if (Vector2.Distance(ally.transform.position, transform.position) < AuraRange)
                {
                    float healPulseAmount = ally.TotalMaxHitpoints * HealPulsePercentage;
                    if (ally == SelfCombatant)
                    {
                        healPulseAmount *= SelfHealingModifier;
                    }
                    ally.HealDamage(healPulseAmount, SelfCombatant);
                }
            }
        }
        /// <summary>
        /// <inheritdoc/> Starts the first healing pulse and starts the aura visual effect. 
        /// Also, one tutorial step depend on launching the healing aura, so if the tutorial is present, we alert it that it should move on with the tutorial.
        /// </summary>
        protected override void OnPersonalSkillStarted()
        {
            AuraInstance.SetActive(true);
            HealPulse();
            var selfSkillTutorial = FindObjectOfType<TutorialStepSelfSkills>();
            if (selfSkillTutorial != null)
            {
                selfSkillTutorial.HealingAuraUsed();
            }
        }
        /// <summary>
        /// Deactivates the aura effect and stops using the aura.
        /// </summary>
        protected override void OnPersonalSkillStopped()
        {
            AuraInstance.SetActive(false);
            timeToNextPulse = float.PositiveInfinity;
        }
    }
}