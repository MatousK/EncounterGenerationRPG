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
    public class CharacterVoiceController : MonoBehaviour
    {
        public CharacterVoiceSounds CombatantSounds;
        public float MajorDamageThreshold = 0.2f;
        private CombatantBase representedCombatant;
        private CombatantsManager combatantsManager;
        private bool IsHero => representedCombatant is Hero;
        private AudioSource audioSource;
        private bool didPlayBloodiedClip;
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

        private void OnDestroy()
        {
            combatantsManager.CombatStarted -= OnCombatStarted;
            combatantsManager.CombatOver -= OnCombatEnded;
        }

        public void PlayOnSelectedSound()
        {
            if (!combatantsManager.IsCombatActive)
            {
                PlaySoundFromList(CombatantSounds.SelectedSounds, true);
            }
        }

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

        private void OnCombatStarted(object sender, EventArgs e)
        {
            didPlayBloodiedClip = false;
        }

        private void OnCombatEnded(object sender, EventArgs e)
        {
            // TODO: Figure out a better way to select who will play the clip.
            if (representedCombatant == combatantsManager.PlayerCharacters.FirstOrDefault())
            {
                PlaySoundFromList(CombatantSounds.FightOverSounds);
            }
        }

        private void CombatantHurt(object sender, int damageTaken)
        {
            if (representedCombatant.IsDown)
            {
                PlaySoundFromList(CombatantSounds.CombatantDiedSounds, isImportant: true);
                return;
            }
            if (representedCombatant.HitPoints <= 0 && IsHero && !didPlayBloodiedClip)
            {
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

        private void CombatantHealed(object sender, int e)
        {
            // Do not play for regeneration.
            if (!combatantsManager.IsCombatActive)
            {
                return;
            }
            PlaySoundFromList(CombatantSounds.HealedSounds);
        }

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
