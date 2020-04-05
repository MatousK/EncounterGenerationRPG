using System;

namespace Assets.Scripts.Combat.Skills.Character.Cleric
{
    class HealOther : TargetedGestureSkill
    {
        public float HealPercentage = 100;
        // Start is called before the first frame update
        protected override void Awake()
        {
            base.Awake();
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
        }
        protected override void ApplySkillEffects(object sender, EventArgs e)
        {
            Target.HealDamage(HealPercentage * Target.TotalMaxHitpoints, SelfCombatant);
        }
    }
}