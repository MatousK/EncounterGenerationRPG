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
    /// <summary>
    /// This component pretty much servers as an intermediary between the UI and the game input controllers.
    /// When a skill is clicked in UI, we set it here to change the behavior of the controllers.
    /// And the <see cref="LeftClickController"/> and <see cref="RightClickController"/> can clear the skill being used if the player clicks somewhere else.
    /// </summary>
    class SkillFromUiIconClickController: MonoBehaviour
    {
        /// <summary>
        /// The targeted skill being used right now.
        /// It must be targeted, as self skill would be used directly, no need for this component.
        /// </summary>
        public TargetedSkill TargetedSkill { get; private set; }
        /// <summary>
        /// The hero using the skill.
        /// </summary>
        public Hero CastingHero { get; protected set; }
        /// <summary>
        /// True if the skill being used is a friendly skill.
        /// </summary>
        public bool IsFriendlySkill;
        /// <summary>
        /// If true, we are currently using a skill from UI.
        /// </summary>
        public bool IsUsingSkill => TargetedSkill != null;
        /// <summary>
        /// Set the skill currently being used from UI.
        /// </summary>
        /// <param name="hero">Hero casting the skill.</param>
        /// <param name="targetedSkill">The skill being used.</param>
        public void SetUsedSkill(Hero hero, TargetedSkill targetedSkill)
        {
            TargetedSkill = targetedSkill;
            CastingHero = hero;
        }
        /// <summary>
        /// Stop trying using the skill from UI. The player canceled the skill usage or used the skill.
        /// </summary>
        public void ClearUsedSkill()
        {
            TargetedSkill = null;
            CastingHero = null;
        }
    }
}
