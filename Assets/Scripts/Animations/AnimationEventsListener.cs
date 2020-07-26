using System;
using UnityEngine;

namespace Assets.Scripts.Animations
{
    /// <summary>
    /// Several of our animations in the game call events. This helps us to e.g. apply effects of an attack at some precise moment in the animation.
    /// However, these animations can call only a single method. Therefore the combatant object has this component attached that provides events
    /// for all of these methods. The animation calls the methods on this helper component, which then raise the appropriate events for the
    /// rest of the components on the combatant.
    /// </summary>
    public class AnimationEventsListener : MonoBehaviour
    {
        /// <summary>
        /// Event raised when an animation reaches a point where the effect of the skill should be applied.
        /// </summary>
        public event EventHandler ApplySkillEffect;
        /// <summary>
        /// Event raised when a skill animation finishes.
        /// </summary>
        public event EventHandler SkillAnimationFinished;
        /// <summary>
        /// This event is raised whenever a sound effect should be played.
        /// </summary>
        public event EventHandler<SoundEffectType> PlaySoundEffectRequested;
        /// <summary>
        /// This event is called from animations to raise the ApplySkillEffect event.
        /// </summary>
        public void AnimationCallbackApplySkillEffect()
        {
            ApplySkillEffect?.Invoke(this, new EventArgs());
        }
        /// <summary>
        /// This event is called from animations to raise the SkillAnimationFinished event.
        /// </summary>
        public void AnimationCallbackSkillAnimationFinished()
        {
            SkillAnimationFinished?.Invoke(this, new EventArgs());
        }
        /// <summary>
        /// This event is called from animations to raise the OnSoundCompleted event.
        /// It is not named correctly, unfortunately renaming it would require refactoring all animations, for which there is no time right now.
        /// </summary>
        public void OnSoundCompleted(SoundEffectType soundEffect)
        {
            PlaySoundEffectRequested?.Invoke(this, soundEffect);
        }
    }
    /// <summary>
    /// Type of sound effect that can be played.
    /// </summary>
    public enum SoundEffectType
    {
        /// <summary>
        /// Sound effect of a single foot step.
        /// </summary>
        Footstep
    }
}
