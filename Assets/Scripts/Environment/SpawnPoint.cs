using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Combat;
using UnityEngine;

namespace Assets.Scripts.Environment
{
    [ExecuteInEditMode]
    public class SpawnPoint: MonoBehaviour
    {
        public SpawnPointType Type;
        public List<SpawnPointColor> SpawnPointColorList;

        private void Awake()
        {
            var spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            var spriteColor = GetColorForSpawnPoint();
            if (spriteRenderer != null && spriteColor != null)
            {
                spriteRenderer.color = spriteColor.Value;
            }
            else
            {
                spriteRenderer.enabled = false;
            }
        }

        public static SpawnPointType GetSpawnPointTypeForCombatant(CombatantBase combatant)
        {
            if (combatant is Hero hero)
            {
                switch (hero.HeroProfession)
                {
                    case HeroProfession.Cleric:
                        return SpawnPointType.Cleric;
                    case HeroProfession.Knight:
                        return SpawnPointType.Knight;
                    case HeroProfession.Ranger:
                        return SpawnPointType.Ranger;
                }
            }
            else if (combatant is Monster monster)
            {
                switch (monster.Role)
                {
                    case MonsterRole.Lurker:
                    case MonsterRole.Minion:
                    case MonsterRole.Brute:
                        return SpawnPointType.CloseRange;
                    case MonsterRole.Leader:
                    case MonsterRole.Sniper:
                        return SpawnPointType.LongRange;
                }
            }
            throw new ArgumentException("Unknown combatant type passed");
        }

        private Color? GetColorForSpawnPoint()
        {
            return SpawnPointColorList.FirstOrDefault(spawnPointColor => spawnPointColor.Type == Type)?.TypeColor;
        }
    }

    [Serializable]
    public class SpawnPointColor
    {
        public SpawnPointType Type;
        public Color TypeColor;
    }

    public enum SpawnPointType
    {
        MostPowerfulEnemy,
        LongRange,
        CloseRange,
        Ranger, 
        Cleric,
        Knight
    }
}
