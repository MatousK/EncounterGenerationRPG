using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Combat;
using Assets.Scripts.Extension;
using UnityEngine;
using UnityEngine.Experimental.XR;

namespace Assets.Scripts.Sound.CharacterSounds
{
    /// <summary>
    /// Controls voice lines the character might say.
    /// The sounds are only played if registered in <see cref="CombatantSounds"/>
    /// </summary>
    public class CharacterVoiceController : MonoBehaviour
    {
        /// <summary>
        /// All sounds registered for the combatant which he might play.
        /// </summary>
        public CharacterVoiceSounds CombatantSounds;
        /// <summary>
        /// How much damage at once should the combatant receive in order for the damage to be considered a major attack.
        /// </summary>
        public float MajorDamageThreshold = 0.2f;
        /// <summary>
        /// The combatant saying these voice lines.
        /// </summary>
        private CombatantBase representedCombatant;
        /// <summary>
        /// The component which knows about all sound effects.
        /// </summary>
        private CombatantsManager combatantsManager;
        /// <summary>
        /// If true, these voices belong to a hero, not a monster.
        /// </summary>
        private bool IsHero => representedCombatant is Hero;
        /// <summary>
        /// The audio source that will play the voice lines.
        /// </summary>
        private AudioSource audioSource;
        /// <summary>
        /// Bloodied sound line plays once per encounter when character's HP reach 0. If true, we have already played the effect this encounter.
        /// </summary>
        private bool didPlayBloodiedClip;
        /// <summary>
        /// Called before the first Update. Initializes the references to dependencies and subscribes to events.
        /// </summary>
        void Start()
        {
            if (CombatantSounds == null)
            {
                CombatantSounds = ScriptableObject.CreateInstance<CharacterVoiceSounds>();
            }
            representedCombatant = GetComponentInParent<CombatantBase>();
            combatantsManager = FindObjectOfType<CombatantsManager>();
            audioSource = GetComponent<AudioSource>();
            audioSource.loop = false;
            representedCombatant.TookDamage += CombatantHurt;
            representedCombatant.HealedDamage += CombatantHealed;
            combatantsManager.CombatStarted += OnCombatStarted;
            combatantsManager.CombatOver += OnCombatEnded;
        }
        /// <summary>
        /// When destroyed, unsubscrbe from events.
        /// </summary>
        private void OnDestroy()
        {
            combatantsManager.CombatStarted -= OnCombatStarted;
            combatantsManager.CombatOver -= OnCombatEnded;
        }
        /// <summary>
        /// Play the greeting sound effect of this hero.
        /// They are not played in combat, the heroes are to busy to greet the player.
        /// </summary>
        public void PlayOnSelectedSound()
        {
            if (!combatantsManager.IsCombatActive)
            {
                PlaySoundFromList(CombatantSounds.SelectedSounds, true);
            }
        }
        /// <summary>
        /// Player when an order is given to the character.
        /// Will do the confirmation line if there is one registered.
        /// </summary>
        /// <param name="order">Order that was given to this character.</param>
        public void OnOrderGiven(VoiceOrderType order)
        {
            switch (order)
            {
                case VoiceOrderType.Attack:
                    PlaySoundFromList(CombatantSounds.AttackOrderSounds, true);
                    break;
                case VoiceOrderType.EnemySkill:
                    PlaySoundFromList(CombatantSounds.EnemySkillOrderSounds, true);
                    break;
                case VoiceOrderType.FriendlySkill:
                    PlaySoundFromList(CombatantSounds.FriendlySkillOrderSounds, true);
                    break;
                case VoiceOrderType.SelfSkill:
                    PlaySoundFromList(CombatantSounds.PersonalSkillOrderSounds, true);
                    break;
                case VoiceOrderType.Move:
                    PlaySoundFromList(CombatantSounds.MoveOrderSounds, true);
                    break;
            }
        }
        /// <summary>
        /// Plays the sound that should play when the character starts using a skill.
        /// </summary>
        /// <param name="skillVoiceType">Type of skill that was used.</param>
        public void SkillAnimationStartedStarted(SkillVoiceType skillVoiceType)
        {
            switch (skillVoiceType)
            {
                case SkillVoiceType.BasicAttack:
                    PlaySoundFromList(CombatantSounds.AttackGrunts);
                    break;
                case SkillVoiceType.SkillAlternate:
                    PlaySoundFromList(CombatantSounds.AlternateSkillUsedSounds);
                    break;
                case SkillVoiceType.SkillNormal:
                    PlaySoundFromList(CombatantSounds.SkillUsedSounds);
                    break;

            }
        }
        /// <summary>
        /// Plays the sound that should play when the effect of a skill is applied, e.g. sword hitting an enemy.
        /// </summary>
        /// <param name="skillVoiceType">Type of skill that was used.</param>
        public void SkillAnimationEffectApplied(SkillVoiceType skillVoiceType)
        {
            switch (skillVoiceType)
            {
                case SkillVoiceType.SkillAlternate:
                    PlaySoundFromList(CombatantSounds.AlternateEffectAppliedSounds);
                    break;
                case SkillVoiceType.SkillNormal:
                    PlaySoundFromList(CombatantSounds.SkillEffectAppliedSounds);
                    break;

            }
        }
        /// <summary>
        /// Called when the combat started, resets the <see cref="didPlayBloodiedClip"/> sound flag.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">Arguments of the event.</param>
        private void OnCombatStarted(object sender, EventArgs e)
        {
            didPlayBloodiedClip = false;
        }
        /// <summary>
        /// When the combat is over, let one character play the appropriate clip.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">Arguments of the event.</param>
        private void OnCombatEnded(object sender, EventArgs e)
        {
            // TODO: Figure out a better way to select who will play the clip.
            if (representedCombatant == combatantsManager.PlayerCharacters.FirstOrDefault())
            {
                PlaySoundFromList(CombatantSounds.FightOverSounds);
            }
        }
        /// <summary>
        /// Called when the represented combatant was damage. The hero might play a grunt
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="damageTaken">How much damage did the player take.</param>
        private void CombatantHurt(object sender, int damageTaken)
        {
            if (representedCombatant.IsDown)
            {
                // The character died, he should complain about that.
                PlaySoundFromList(CombatantSounds.CombatantDiedSounds, isImportant: true);
                return;
            }
            if (representedCombatant.HitPoints <= 0 && IsHero && !didPlayBloodiedClip)
            {
                // The chracter is really hurt.
                PlaySoundFromList(CombatantSounds.HeroStartsLosingMaxHpSounds, true);
                didPlayBloodiedClip = true;
                return;
            }

            var damagePercentage = ((float)damageTaken) / representedCombatant.TotalMaxHitpoints;
            if (damagePercentage <= MajorDamageThreshold)
            {
                PlaySoundFromList(CombatantSounds.MinorDamageGrunts);
            }
            else
            {
                PlaySoundFromList(CombatantSounds.MajorDamageGrunts);
            }
        }
        /// <summary>
        /// Called when the hero is healed.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Amount for which the combatant was healed.</param>
        private void CombatantHealed(object sender, int e)
        {
            // Do not play for regeneration.
            if (!combatantsManager.IsCombatActive)
            {
                return;
            }
            PlaySoundFromList(CombatantSounds.HealedSounds);
        }
        /// <summary>
        /// Plays a random sound effect from the given list of audio clips.
        /// Will play nothing if the list is empty.
        /// </summary>
        /// <param name="list">List of audio clips that could be played for some character and event.</param>
        /// <param name="isImportant">If true, this line is so important that it should interrupt whatever else is playing.</param>
        private void PlaySoundFromList(IEnumerable<AudioClip> list, bool isImportant = false)
        {
            var clipToPlay = list.GetRandomElementOrDefault();
            if ((audioSource.isPlaying && !isImportant) || clipToPlay == null)
            {
                // We do not want to overlap sounds.
                return;
            }
            audioSource.clip = clipToPlay;
            audioSource.Play();
        }
    }
}
