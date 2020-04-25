using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        MostPowerfulMonster,
        LongRange,
        CloseRange,
        Ranger, 
        Cleric,
        Knight
    }
}
