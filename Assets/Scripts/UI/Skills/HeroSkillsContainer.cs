using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Combat;
using UnityEngine;

namespace Assets.Scripts.UI.Skills
{
    public class HeroSkillsContainer : MonoBehaviour
    {
        public Sprite EnemySkillFrame;
        public Sprite SelfSkillFrame;
        public Sprite FriendlySkillFrame;

        public SkillUiIcon EnemySkillIcon;
        public SkillUiIcon SelfSkillIcon;
        public SkillUiIcon FriendlySkillIcon;

        private Hero representedHero;

        public Hero RepresentedHero
        {
            get => representedHero;
            set
            {
                representedHero = value;
                SetSkillsForIcons();
            }
        }

        private void Start()
        {
            SetSkillsForIcons();
        }


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

        private void SetSkillsForIcons()
        {
            if (RepresentedHero == null)
            {
                return; ;
            }

            EnemySkillIcon.SetSkill(RepresentedHero.EnemyTargetSkill, EnemySkillFrame);
            SelfSkillIcon.SetSkill(RepresentedHero.SelfTargetSkill, SelfSkillFrame);
            FriendlySkillIcon.SetSkill(RepresentedHero.FriendlyTargetSkill, FriendlySkillFrame);
        }
    }
}
