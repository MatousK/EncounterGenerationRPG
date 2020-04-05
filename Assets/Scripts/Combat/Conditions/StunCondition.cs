using Assets.Scripts.Combat.Skills;

namespace Assets.Scripts.Combat.Conditions
{
    /// <summary>
    /// A combatant with this condition cannot take any actions.
    /// </summary>
    public class StunCondition: ConditionBase
    { 
        protected override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {
            base.Update();
        }

        protected override void StartCondition()
        {
            base.StartCondition();
            foreach (var skill in GetComponentsInChildren<Skill>())
            {
                skill.TryStopSkill();
            }
        }
    }
}