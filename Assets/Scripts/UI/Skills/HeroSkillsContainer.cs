using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Combat;
using UnityEngine;

namespace Assets.Scripts.UI.Skills
{
    /// <summary>
    /// Component for the container which can show all skills of some hero.
    /// </summary>
    public class HeroSkillsContainer : MonoBehaviour
    {
        /// <summary>
        /// The frame to be drawn around enemy skills.
        /// </summary>
        public Sprite EnemySkillFrame;
        /// <summary>
        /// The frame to be drawn around self skills.
        /// </summary>
        public Sprite SelfSkillFrame;
        /// <summary>
        /// The frame to be drawn around friendly skill.
        /// </summary>
        public Sprite FriendlySkillFrame;
        /// <summary>
        /// The component which controls the enemy skill icon.
        /// </summary>
        public SkillUiIcon EnemySkillIcon;
        /// <summary>
        /// The component which controls the self skill icon.
        /// </summary>
        public SkillUiIcon SelfSkillIcon;
        /// <summary>
        /// The component which controls the friendly skill icon.
        /// </summary>
        public SkillUiIcon FriendlySkillIcon;
        /// <summary>
        /// The hero whose skills this container is showing.
        /// Internal, do not use, use <see cref="RepresentedHero"/>
        /// </summary>
        private Hero representedHero;
        /// <summary>
        /// The hero whose skills this container is showing.
        /// When changed, also changes the skills shown in the skill icons.
        /// </summary>
        public Hero RepresentedHero
        {
            get => representedHero;
            set
            {
                representedHero = value;
                SetSkillsForIcons();
            }
        }
        /// <summary>
        /// Called before first update. Updates the skills assigned to individual icons.
        /// </summary>
        private void Start()
        {
            SetSkillsForIcons();
        }

        /// <summary>
        /// Called every frame. Updates the cooldown indicators on skills.
        /// </summary>
        private void Update()
        {
            if (RepresentedHero == null)
            {
                return;
            }

            var cooldownRemaining = RepresentedHero.LastSkillRemainingCooldown ?? 0;
            var cooldownTotal = RepresentedHero.LastSkillCooldown;
            var cooldownPercentage = cooldownRemaining / cooldownTotal ?? 0;
            cooldownPercentage = cooldownPercentage >= 0 ? cooldownPercentage : 0;

            EnemySkillIcon.CurrentCooldownPercentage = cooldownPercentage;
            SelfSkillIcon.CurrentCooldownPercentage = cooldownPercentage;
            FriendlySkillIcon.CurrentCooldownPercentage = cooldownPercentage;
        }
        /// <summary>
        /// Sets the skills each of the icons represents.
        /// </summary>
        private void SetSkillsForIcons()
        {
            if (RepresentedHero == null)
            {
                return;
            }

            EnemySkillIcon.SetSkill(RepresentedHero.EnemyTargetSkill, EnemySkillFrame);
            SelfSkillIcon.SetSkill(RepresentedHero.SelfTargetSkill, SelfSkillFrame);
            FriendlySkillIcon.SetSkill(RepresentedHero.FriendlyTargetSkill, FriendlySkillFrame);
        }
    }
}
