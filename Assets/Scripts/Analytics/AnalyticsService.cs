using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Combat;
using Assets.Scripts.EncounterGenerator.Model;
using Assets.Scripts.GameFlow;
using UnityEngine;

namespace Assets.Scripts.Analytics
{
    public class AnalyticsService : MonoBehaviour
    {
        public Guid UserGuid = Guid.NewGuid();
        public bool IsPendingKill;
        private List<MonsterType> orderedMonsterTypes = new List<MonsterType>
        {
            new MonsterType(MonsterRank.Minion, MonsterRole.Minion),
            new MonsterType(MonsterRank.Regular, MonsterRole.Brute),
            new MonsterType(MonsterRank.Elite, MonsterRole.Brute),
            new MonsterType(MonsterRank.Boss, MonsterRole.Brute),
            new MonsterType(MonsterRank.Regular, MonsterRole.Leader),
            new MonsterType(MonsterRank.Elite, MonsterRole.Leader),
            new MonsterType(MonsterRank.Boss, MonsterRole.Leader),
            new MonsterType(MonsterRank.Regular, MonsterRole.Lurker),
            new MonsterType(MonsterRank.Elite, MonsterRole.Lurker),
            new MonsterType(MonsterRank.Boss, MonsterRole.Lurker),
            new MonsterType(MonsterRank.Regular, MonsterRole.Sniper),
            new MonsterType(MonsterRank.Elite, MonsterRole.Sniper),
            new MonsterType(MonsterRank.Boss, MonsterRole.Sniper),
        };

        void Awake()
        {
            if (FindObjectsOfType<AnalyticsService>().Length > 1)
            {
                IsPendingKill = true;
                Destroy(this);
                return;
            }
            DontDestroyOnLoad(gameObject);
            UnityEngine.Analytics.Analytics.SetUserId(UserGuid.ToString());
        }
        /// <summary>
        /// Used to reset guid once the player finishes the game.
        /// </summary>
        public void ResetGuid()
        {
            UserGuid = Guid.NewGuid();
        }

        public void LogScreenVisit(SceneType scene, int levelIndex = 0)
        {
        }

        public void LogCombat(Dictionary<HeroProfession, float> partyStartHp, Dictionary<HeroProfession, float> partyEndHp, Dictionary<HeroProfession, float> partyAttack,
            EncounterDefinition encounter, float expectedDifficulty, float realDifficulty, bool wasGameOver, bool wasStatic)
        {
            var args = new Dictionary<string, object>
            {
                {"start_max_hp", CollapseToString(partyStartHp)},
                {"end_max_hp", CollapseToString(partyEndHp)},
                {"attack", CollapseToString(partyAttack)},
                {"expected_difficulty", expectedDifficulty},
                {"real_difficulty", realDifficulty},
                {"was_game_over", wasGameOver},
                {"was_static", wasStatic},
                {"encounter", GetEncounterString(encounter)}
            };
            var analyticsResult =  UnityEngine.Analytics.Analytics.CustomEvent(
                "combat_over", args

            );
            print(args);
            print(analyticsResult);
        }

        private string GetEncounterString(EncounterDefinition encounter)
        {
            string toReturn = "";
            foreach (var monsterType in orderedMonsterTypes)
            {
                var monsterCount =
                    encounter.AllEncounterGroups.FirstOrDefault(group => group.MonsterType == monsterType)?.MonsterCount ?? 0;
                toReturn += monsterCount.ToString();
                toReturn += ";";
            }

            return toReturn;
        }

        private string CollapseToString(Dictionary<HeroProfession, float> dictionary)
        {
            return string.Join(";",
                dictionary.Select(keyValue => keyValue.Key + ":" + (int)keyValue.Value));
        }


    }
}