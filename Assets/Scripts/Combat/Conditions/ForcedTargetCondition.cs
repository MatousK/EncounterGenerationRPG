using System.Linq;
using Assets.Scripts.Combat.Skills;

namespace Assets.Scripts.Combat.Conditions
{
    /// <summary>
    /// Helper condition for AI - specifies that the affected AI MUST attack the specified player or character for a specified duration.
    /// When another target is called, the most recently applied condition will apply.
    /// </summary>
    class ForcedTargetCondition : ConditionBase
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

        protected override void StartCondition()
        {
            base.StartCondition();
            // If currently targeting someone, try to shift the target to the forced target.
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