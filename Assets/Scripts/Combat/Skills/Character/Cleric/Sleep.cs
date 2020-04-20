using System;
using Assets.Scripts.Combat.Conditions;

namespace Assets.Scripts.Combat.Skills.Character.Cleric
{
    class Sleep : TargetedGestureSkill
    {
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
        protected override void ApplySkillEffects(object sender, EventArgs e)
        {
            var sleepCondition = Target.GetComponent<ConditionManager>().AddCondition<SleepCondition>();
            sleepCondition.RemainingDuration = SleepDuration;
            base.ApplySkillEffects(sender, e);
        }
    }
}