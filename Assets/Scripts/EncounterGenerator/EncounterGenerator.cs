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
        var leaderMonsterTypeDefinition = new MonsterTypeDefinition(MonsterRank.Elite, MonsterRole.Leader);
        var bruteMonsterTypeDefinition = new MonsterTypeDefinition(MonsterRank.Regular, MonsterRole.Brute);
        var sniperMonsterTypeDefinition = new MonsterTypeDefinition(MonsterRank.Regular, MonsterRole.Sniper);
        var minionMonsterTypeDefinition = new MonsterTypeDefinition(MonsterRank.Regular, MonsterRole.Minion);
        var lurkerMonsterTypeDefinition = new MonsterTypeDefinition(MonsterRank.Regular, MonsterRole.Lurker);
        generationParams.RequestedMonsters = new List<KeyValuePair<MonsterTypeDefinition, int>> {
            new KeyValuePair<MonsterTypeDefinition, int>(bruteMonsterTypeDefinition, (int)targetDifficulty-1),
        //    new KeyValuePair<MonsterTypeDefinition, int>(leaderMonsterTypeDefinition, 1),
            new KeyValuePair<MonsterTypeDefinition, int>(sniperMonsterTypeDefinition, 1),
            new KeyValuePair<MonsterTypeDefinition, int>(minionMonsterTypeDefinition, 4),
            new KeyValuePair<MonsterTypeDefinition, int>(lurkerMonsterTypeDefinition, 1)
            };
        return configuration.MonsterGroupDefinition.First().GenerateMonsterGroup(generationParams);
    }

}