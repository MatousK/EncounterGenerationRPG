using System.Linq;
using Assets.Scripts.Combat;
using Assets.Scripts.Combat.Conditions;

namespace Assets.Scripts.AI.HeroAI
{
    /// <summary>
    /// Base class for Hero artifical intelligence.
    /// Note that these are used only for the simulator to fill the difficulty matrix, not used during gameplay
    /// That is why there was not much emphasis to make the hero AIs universal or especially good, they simply must display some basic level of competence.
    /// This class and its children are only meant to be used in the combat simulator.
    /// </summary>
    public class HeroAiBase : AiBase
    {
        /// <summary>
        /// Reference to the knight character in the party.
        /// </summary>
        public Hero Knight;
        /// <summary>
        /// Reference to the ranger character in the party.
        /// </summary>
        public Hero Ranger;
        /// <summary>
        /// Reference to the cleric character in the party.
        /// </summary>
        public Hero Cleric;
        /// <summary>
        /// The hero this AI controls.
        /// </summary>
        protected Hero ControlledHero => (Hero)ControlledCombatant;
        protected override void Update()
        {
            base.Update();
        }

        protected override void Start()
        {
            base.Start();
        }
        /// <summary>
        /// Called when AI is requested to do an action.
        /// Child classes should override it.
        /// Basic implementation for hero tries to attack the most dangerous target.
        /// If the AI thinks it is stuck (which happens when tha character cannot get in range to use the skill), it instead attacks the closest target.
        /// </summary>
        /// <returns>True if the attack was successful.</returns>
        protected override bool TryDoAction()
        {
            var target = IsProbablyStuck ? GetClosestOpponent() : GetMostDangerousTarget();
            return TryUseSkill(target, BasicAttack);
        }
        /// <summary>
        /// Returns the most dangerous target that fulfills some criteria.
        /// </summary>
        /// <param name="dangerousnessThreshold"> If set, this specifies the minimum dangerousness of the enemy returned.</param>
        /// <param name="includeTargetOfSleepSkill"> If true, the skill will consider all enemies, even those who are asleep or about to be put to sleep by the cleric. Default false.</param>
        /// <returns>The most dangerous monster that fits the criteria. If no monster fits the criteria, returns null.</returns>
        protected Monster GetMostDangerousTarget(float dangerousnessThreshold = 0f, bool includeTargetOfSleepSkill = false)
        {
            Monster toReturn;
            var allEnemies = CombatantsManager.GetEnemies(onlyAlive: true);
            var orderedEnemies = allEnemies.OrderBy(GetMonsterDangerScore).ToList();
            if (orderedEnemies.Count < 2)
            {
                toReturn = orderedEnemies.FirstOrDefault();
            }
            else
            {
                // If includeTargetOfSleepSkill is false, we want to return the second most dangerous enemy in case either the most dangerous one is being to put to sleep or is already asleep.
                if (!includeTargetOfSleepSkill && orderedEnemies.Count >= 2 &&
                    ((Cleric.EnemyTargetSkill.IsBeingUsed() && Cleric.EnemyTargetSkill.Target == orderedEnemies.Last()) ||
                     orderedEnemies.Last().GetComponent<ConditionManager>().HasCondition<SleepCondition>()))
                {
                    toReturn = orderedEnemies[orderedEnemies.Count - 2];
                }
                else
                {
                    toReturn = orderedEnemies.LastOrDefault();
                }
            }
            if (toReturn != null && GetMonsterDangerScore(toReturn) >= dangerousnessThreshold)
            {
                return toReturn;
            }
            return null;
        }
        /// <summary>
        /// Calculates a danger score for the monster, which determines how much should the heroes try to kill it. This is used only by the AI.
        /// </summary>
        /// <param name="monster">The monster whose danger score is requested.</param>
        /// <returns>The danger score of the monster.</returns>
        protected float GetMonsterDangerScore(Monster monster)
        {
            float rankDanger = 0;
            float roleDanger = 0;
            switch (monster.Role)
            {
                case MonsterRole.Brute:
                    roleDanger = 1;
                    break;
                case MonsterRole.Minion:
                    roleDanger = 0;
                    break;
                case MonsterRole.Sniper:
                    roleDanger = 1.2f;
                    break;
                case MonsterRole.Leader:
                    roleDanger = 1.5f;
                    break;
                case MonsterRole.Lurker:
                    roleDanger = 3;
                    break;
            }

            switch (monster.Rank)
            {
                case MonsterRank.Minion:
                    rankDanger = 0;
                    break;
                case MonsterRank.Regular:
                    rankDanger = 1;
                    break;
                case MonsterRank.Elite:
                    rankDanger = 2;
                    break;
                case MonsterRank.Boss:
                    rankDanger = 4;
                    break;
            }

            return rankDanger * roleDanger;
        }
    }
}