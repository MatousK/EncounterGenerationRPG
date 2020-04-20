using System;
using Assets.Scripts.Combat.Conditions;
using Assets.Scripts.Effects;
using Assets.Scripts.Movement;
using Assets.Scripts.Movement.Pathfinding;
using UnityEngine;

namespace Assets.Scripts.Combat.Skills.Character.Knight
{
    public class ToTheRescue : TargetedSkill
    {
        public float TauntRange = 3;
        public float TauntDuration = 5;
        public float MovementSpeedMultiplier = 5;
        private PathfindingMapController pathfindingMapController;
        private CombatantsManager combatantsManager;
        private Grid mapGrid;
        public ToTheRescue()
        {
            Speed = 2;
            Range = float.MaxValue;
            SkillAnimationName = "Walking";
        
        }

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            pathfindingMapController = FindObjectOfType<PathfindingMapController>();
            mapGrid = FindObjectOfType<Grid>();
            combatantsManager = FindObjectOfType<CombatantsManager>();
        }

        public override bool UseSkillOn(CombatantBase target)
        {
            var targetSquare = GetTargetSquare(target);
            if (targetSquare == null)
            {
                return false;
            }
            // Also check other preconditions in parent , like cooldown etc.
            var toReturn = base.UseSkillOn(target);
            if (toReturn)
            {
                ExecuteSkill(targetSquare.Value);
            }
            return toReturn;
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
        }

        protected override void ApplySkillEffects(object sender, EventArgs e) 
        {
            // Do nothing, as this skill works differently fron other ones and is independent on animations.
        }

        private void ExecuteSkill(Vector2Int targetPosition)
        {
            SelfCombatant.Attributes.MovementSpeedMultiplier *= MovementSpeedMultiplier;
            Vector2 targetWorldSpace = mapGrid.CellToWorld((Vector3Int)targetPosition);
            SelfCombatant.GetComponent<MovementController>().MoveToPosition(targetWorldSpace, ignoreOtherCombatants: true, onMoveToSuccessful: MoveToSuccessful);
        }

        private void MoveToSuccessful(bool result)
        {
            SelfCombatant.Attributes.MovementSpeedMultiplier /= MovementSpeedMultiplier;
            SelfCombatant.GetComponentInChildren<TauntEffect>().StartEffect();
            foreach (var enemy in combatantsManager.GetEnemies(onlyAlive: true))
            {
                if (Vector2.Distance(enemy.transform.position, SelfCombatant.transform.position) > TauntRange)
                {
                    continue;
                }
                var tauntCondition = enemy.GetComponent<ConditionManager>().AddCondition<ForcedTargetCondition>();
                tauntCondition.RemainingDuration = TauntDuration;
                tauntCondition.ForcedTarget = SelfCombatant;
                tauntCondition.TargetForcedBy = SelfCombatant;
            }
            TryStopSkill();
        }

        private Vector2Int? GetTargetSquare(CombatantBase target)
        {
            return pathfindingMapController.GetPassableSpaceInDistance(SelfCombatant, (Vector2Int)mapGrid.WorldToCell(target.transform.position), 1);
        }

        protected override void AnimationCompleted(object sender, EventArgs e)
        {
            // do nothing, we just do not want the base implementation to stop the skill prematurely.
        }
    }
}
