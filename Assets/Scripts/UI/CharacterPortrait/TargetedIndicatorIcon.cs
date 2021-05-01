using Assets.Scripts.Combat;
using Assets.Scripts.Combat.Conditions;
using Assets.Scripts.UI.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.UI.CharacterPortrait
{
    public class TargetedIndicatorIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public float ToolTipAppearDelay;
        public ToolTip ToolTip;
        private float? pointerEnterTime;
        private CombatantsManager combatantsManager;
        [HideInInspector]
        public Hero RepresentedHero;
        CombatantBase lastFrameTargetCaster;
        [TextArea(0, 10)]
        public string DescriptionTextFormat;
        public string EnemySingularFormat;
        public string EnemyPluralFormat;
        public string Description;
        public DurationIndicator DurationIndicator;

        private void Start()
        {
            combatantsManager = FindObjectOfType<CombatantsManager>();
        }

        private void Update()
        {
            var enemies = combatantsManager.GetEnemies(onlyAlive: true);
            var targetedByCount = 0;
            var largestRemainingDuration = 0f;
            CombatantBase targetCaster = null;
            foreach (var enemy in enemies)
            {
                var enemyConditions = enemy.GetComponentInChildren<ConditionManager>().ActiveConditions;
                var forcedTargetCondition = enemyConditions.FirstOrDefault((condition) => condition is ForcedTargetCondition) as ForcedTargetCondition;
                if (forcedTargetCondition == null || forcedTargetCondition.ForcedTarget != RepresentedHero)
                {
                    continue;
                }
                ++targetedByCount;
                if (targetCaster == null || forcedTargetCondition.TargetForcedBy != RepresentedHero)
                {
                    // Ok, hacky as hell. Basically we assume that there is only one enemy who can do targets, the cleric.
                    // The rest of targets are done by the knight. And in the rare case when the target is done both by the knight and enemies, well...
                    // We assume it is more important to know that the enemy targetted the character. Not that this matters much, because by the point
                    // the knight is targetted, he is the only one alive.
                    targetCaster = forcedTargetCondition.TargetForcedBy;
                }
                if (forcedTargetCondition.RemainingDuration > largestRemainingDuration)
                {
                    DurationIndicator.RemainingDuration = forcedTargetCondition.RemainingDuration;
                    DurationIndicator.TotalDuration = forcedTargetCondition.TotalDuration;
                }
            }
            if (targetCaster != lastFrameTargetCaster)
            {
                if (targetCaster == null)
                {
                    GetComponent<Animation>().Play("ConditionDisappearAnimation");
                } 
                else
                {
                    GetComponent<Animation>().Play("ConditionAppearAnimation");
                }
            }
            lastFrameTargetCaster = targetCaster;
            if (targetCaster != null)
            {
                UpdateDescription(targetedByCount, targetCaster);
            }

            if (pointerEnterTime != null && Time.unscaledTime - pointerEnterTime.Value > ToolTipAppearDelay)
            {
                ToolTip.IsVisible = true;
            }
            ToolTip.Text = Description;
        }

        private void UpdateDescription(int enemyCount, CombatantBase caster)
        {
            var enemyCountFormat = enemyCount == 1 ? EnemySingularFormat : EnemyPluralFormat;
            var enemyCountText = string.Format(enemyCountFormat, enemyCount);
            Description = string.Format(DescriptionTextFormat, enemyCountText, caster.HumanFriendlyName);
        }

        /// <summary>
        /// Called when the pointer enters the indicator. Will start the timer that will eventually show tool tip.
        /// Called automatically by Unity.
        /// </summary>
        /// <param name="eventData">The data about the pointer which entered the icon.</param>
        public void OnPointerEnter(PointerEventData eventData)
        {
            pointerEnterTime = Time.unscaledTime;
        }
        /// <summary>
        /// Called when the pointer leaves this skill icon. Will stop the timer that might have showed the skill name.
        /// If the skill name was shown, hide it.
        /// Called automatically by Unity.
        /// </summary>
        /// <param name="eventData">The data about the pointer which left the icon.</param>
        public void OnPointerExit(PointerEventData eventData)
        {
            pointerEnterTime = null;
            ToolTip.IsVisible = false;
        }
    }
}
