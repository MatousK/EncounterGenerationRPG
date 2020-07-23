using System;
using Assets.Scripts.Tutorial;

namespace Assets.Scripts.Combat.Skills.Character.Cleric
{
    /// <summary>
    /// This skill heals the target ally for some amount.
    /// </summary>
    class HealOther : TargetedGestureSkill
    {
        /// <summary>
        /// In percentage, how many HP should this skill heal.
        /// </summary>
        public float HealPercentage = 100;
        // Start is called before the first frame update
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
        /// <inheritdoc/> Heals the target for the <see cref="HealPercentage"/> of his total max hit points. 
        /// Also, one tutorial depends on this skill, so we also try to find the tutorial game object and tell it that this skill was used.
        /// </summary>
        /// <param name="sender">Sender of this event.</param>
        /// <param name="e">ARguments of this event.</param>
        protected override void ApplySkillEffects(object sender, EventArgs e)
        {
            Target.HealDamage(HealPercentage * Target.TotalMaxHitpoints, SelfCombatant);
            base.ApplySkillEffects(sender, e);
            var tutorialStep = FindObjectOfType<TutorialStepAllySkills>();
            if (tutorialStep != null)
            {
                tutorialStep.HealOtherUsed();
            }
        }
    }
}