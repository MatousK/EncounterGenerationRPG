using System;

namespace Assets.Scripts.Combat.Skills.Monster.Leader
{
    public class HealLightWounds : TargetedGestureSkill
    {
        public int HealAmount;
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
            Target.HealDamage(HealAmount, SelfCombatant);
            base.ApplySkillEffects(sender, e);
        }
    }
}
