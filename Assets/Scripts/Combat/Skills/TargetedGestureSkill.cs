namespace Assets.Scripts.Combat.Skills
{
    /// <summary>
    /// A targeted skill which use Gesture as its skill animation, e.g. spells.
    /// </summary>
    public abstract class TargetedGestureSkill : TargetedSkill
    {
        protected TargetedGestureSkill()
        {
            SkillAnimationName = "Gesturing";
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {
            base.Update();
        }
    }
}