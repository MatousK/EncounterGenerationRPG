using System;
using Assets.Scripts.Combat.Conditions;

namespace Assets.Scripts.Combat.Skills.Character.Cleric
{
    /// <summary>
    /// Puts the target to sleep for some duration, see <see cref="SleepDuration"/>.
    /// </summary>
    class Sleep : TargetedGestureSkill
    {
        /// <summary>
        /// How long should the target remend asleep.
        /// </summary>
        public float SleepDuration = 10;
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
        /// <inheritdoc/> Puts the target to sleep, see <see cref="SleepCondition"/>.
        /// </summary>
        /// <param name="sender">Sender of this event.</param>
        /// <param name="e">Arguments of the event.</param>
        protected override void ApplySkillEffects(object sender, EventArgs e)
        {
            var sleepCondition = Target.GetComponent<ConditionManager>().AddCondition<SleepCondition>();
            sleepCondition.RemainingDuration = SleepDuration;
            base.ApplySkillEffects(sender, e);
        }
    }
}