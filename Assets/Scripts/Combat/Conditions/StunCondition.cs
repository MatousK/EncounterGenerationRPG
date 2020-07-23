using Assets.Scripts.Combat.Skills;

namespace Assets.Scripts.Combat.Conditions
{
    /// <summary>
    /// For whatever reason this character can do no actions.
    /// Stop everything the character was doing. AI will ensure this combatant does nothing.
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
        /// <summary>
        /// Stop doing everything. AI checks for this condition. If the controlled combatant is stunned, it does nothing.
        /// </summary>
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