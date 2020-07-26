namespace Assets.Scripts.Combat.Skills
{
    /// <summary>
    /// A targeted skill which use Gesture as its skill animation, e.g. spells.
    /// </summary>
    public abstract class TargetedGestureSkill : TargetedSkill
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TargetedGestureSkill"/> class.
        /// </summary>
        protected TargetedGestureSkill()
        {
            SkillAnimationName = "Gesturing";
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void Start()
        {
            base.Start();
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void Update()
        {
            base.Update();
        }
    }
}