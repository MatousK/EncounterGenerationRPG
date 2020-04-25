using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Combat;
using Assets.Scripts.Combat.Skills;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Input
{
    class SkillFromUiIconClickController: MonoBehaviour
    {
        public TargetedSkill TargetedSkill { get; private set; }
        public Hero CastingHero { get; protected set; }
        public bool IsFriendlySkill;
        public bool IsUsingSkill => TargetedSkill != null;
        public void SetUsedSkill(Hero hero, TargetedSkill targetedSkill)
        {
            TargetedSkill = targetedSkill;
            CastingHero = hero;
        }

        public void ClearUsedSkill()
        {
            TargetedSkill = null;
            CastingHero = null;
        }
    }
}
