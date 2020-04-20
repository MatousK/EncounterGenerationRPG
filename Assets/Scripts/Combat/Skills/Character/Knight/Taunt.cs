using Assets.Scripts.Combat.Conditions;
using Assets.Scripts.Effects;

namespace Assets.Scripts.Combat.Skills.Character.Knight
{
    public class Taunt : PersonalSkill
    {
        public float ReceivedDamageModifier = 0.5f;
        public float TauntDuration;
        private CombatantsManager combatantsManager;
        public Taunt()
        {
            SkillAnimationName = "";

        }
        protected override void OnPersonalSkillStarted()
        {
            SelfCombatant.GetComponentInChildren<TauntEffect>().StartEffect();
            SelfCombatant.Attributes.ReceivedDamageMultiplier *= ReceivedDamageModifier;
            foreach (var enemy in combatantsManager.GetEnemies(onlyAlive: true))
            {
                var tauntCondition = enemy.GetComponent<ConditionManager>().AddCondition<ForcedTargetCondition>();
                tauntCondition.RemainingDuration = TauntDuration;
                tauntCondition.ForcedTarget = SelfCombatant;
                tauntCondition.TargetForcedBy = SelfCombatant;
            }
        }

        protected override void OnPersonalSkillStopped()
        {
            SelfCombatant.Attributes.ReceivedDamageMultiplier /= ReceivedDamageModifier;
        }

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            combatantsManager = FindObjectOfType<CombatantsManager>();
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
        }
    }
}
