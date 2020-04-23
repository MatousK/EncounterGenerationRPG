using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Combat.Skills;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Skills
{
    public class SkillUiIcon: MonoBehaviour
    {
        public Image SkillIcon;

        public Image Frame;

        public Image CooldownOverlay;

        public float CurrentCooldownPercentage;

        private Skill representedSkill;

        private void Update()
        {
            CooldownOverlay.transform.localScale = new Vector3(1, CurrentCooldownPercentage,1);
        }

        public void SetSkill(Skill skill, Sprite frameSprite)
        {
            representedSkill = skill;

            SkillIcon.sprite = skill.SkillIcon;

            Frame.sprite = frameSprite;
        }
    }
}
