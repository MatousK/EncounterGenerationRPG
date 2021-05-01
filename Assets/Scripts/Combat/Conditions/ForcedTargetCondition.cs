using System.Linq;
using Assets.Scripts.Combat.Skills;

namespace Assets.Scripts.Combat.Conditions
{
    /// <summary>
    /// Helper condition for AI - specifies that the AI of the affected combatant MUST attack the specified player or character for a specified duration.
    /// When another target is called, the most recently applied condition will apply.
    /// </summary>
    public class ForcedTargetCondition : ConditionBase
    {
        /// <summary>
        /// The target this character must attack at all cost.
        /// </summary>
        public CombatantBase ForcedTarget;
        /// <summary>
        /// If true, once the character who cast this condition dies the condition will end.
        /// </summary>
        public bool StopTargetingOnceCallerDead = true;
        /// <summary>
        /// The character who forced this target.
        /// </summary>
        public CombatantBase TargetForcedBy;
        /// <summary>
        /// In addition to base behavior, ends the condition if caster dies.
        /// </summary>
        protected override void Update()
        {
            base.Update();
            if (StopTargetingOnceCallerDead && (TargetForcedBy != null && TargetForcedBy.IsDown))
            {
                EndCondition();
            }
        }

        protected override void Start() {
            base.Start();
        }
        /// <summary>
        /// If the combatant was currently moving to the target of a skill, stop that, target changed.
        /// We do not interrupt the AI in the middle of an attack.
        /// The AI will check for this condition everytime it decides what it should be doing.
        /// </summary>
        protected override void StartCondition()
        {
            base.StartCondition();
            var selfCombatant = GetComponent<CombatantBase>();
            var currentSkill = selfCombatant.CombatantSkills.FirstOrDefault(skill => skill.IsBeingUsed()) as TargetedSkill;
            if (currentSkill == null)
            {
                return;;
            }
            // We forced attacker to change targets - if he is not in a middle of an attack, he should stop moving and attack the forced target.
            if (currentSkill.IsMovingToTarget())
            {
                currentSkill.TryStopSkill();
            }

        }
    }
}