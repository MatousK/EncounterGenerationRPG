using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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
        var testMonsterTypeDefinition = new MonsterTypeDefinition(MonsterRank.Regular, MonsterRole.Brute);
        var leaderMonsterTypeDefinition = new MonsterTypeDefinition(MonsterRank.Boss, MonsterRole.Lurker);
        generationParams.RequestedMonsters = new List<KeyValuePair<MonsterTypeDefinition, int>> {
            new KeyValuePair<MonsterTypeDefinition, int>(testMonsterTypeDefinition, (int)targetDifficulty-1),
            new KeyValuePair<MonsterTypeDefinition, int>(leaderMonsterTypeDefinition, 1)
            };
        return configuration.MonsterGroupDefinition.First().GenerateMonsterGroup(generationParams);
    }
}