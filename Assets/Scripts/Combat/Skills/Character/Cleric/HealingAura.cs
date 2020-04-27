using Assets.Scripts.Tutorial;
using UnityEngine;

namespace Assets.Scripts.Combat.Skills.Character.Cleric
{
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
        /// How far must the character be for the aura to work.
        /// </summary>
        public float AuraRange = 0;
        /// <summary>
        ///  How many times per second should the heal pulse fire.
        /// </summary>
        public float HealPulseFrequency = 0;
        /// <summary>
        /// How much healing should the aura do per pulse.
        /// </summary>
        public float HealPulsePercentage = 10;
        /// <summary>
        /// The aura object which should be activated while this skill is active.
        /// </summary>
        public GameObject AuraInstance = null;
        /// <summary>
        /// Returns how much time is between heal pulses.
        /// </summary>
        private float PulseTime => 1 / HealPulseFrequency;

        private float timeToNextPulse = float.PositiveInfinity;
        private CombatantsManager combatantsManager;
        // Start is called before the first frame update
        protected override void Start()
        {
            combatantsManager = FindObjectOfType<CombatantsManager>();
            base.Start();
        }

        // Update is called once per frame
        protected override void Update()
        {
            timeToNextPulse -= Time.deltaTime;
            if (timeToNextPulse <= 0)
            {
                HealPulse();
            }
            base.Update();
        }

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
                        healPulseAmount /= SelfHealingModifier;
                    }
                    ally.HealDamage(healPulseAmount, SelfCombatant);
                }
            }
        }

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

        protected override void OnPersonalSkillStopped()
        {
            AuraInstance.SetActive(false);
            timeToNextPulse = float.PositiveInfinity;
        }
    }
}