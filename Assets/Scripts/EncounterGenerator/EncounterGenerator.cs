using EncounterGenerator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EncounterGenerator
{
    public class EncounterGenerator
    {
        public List<GameObject> GenerateEncounters(EncounterConfiguration configuration)
        {
            var targetDifficulty = configuration.EncounterDifficulty.GetDifficultyForPartyStrength(0);
            var generationParams = new GenerateMonsterGroupParameters
            {
                TargetEncounterDifficulty = targetDifficulty,
                MonsterPriorities = new Dictionary<GameObject, float>()
            };
            var testMonsterTypeDefinition = new MonsterType(MonsterRank.Regular, MonsterRole.Brute);
            var leaderMonsterTypeDefinition = new MonsterType(MonsterRank.Boss, MonsterRole.Lurker);
            generationParams.RequestedMonsters = new EncounterDefinition();
            return configuration.MonsterGroupDefinition.First().GenerateMonsterGroup(generationParams);
        }
    }
}