using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Combat;
using UnityEngine;

namespace Assets.Scripts.Environment
{
    /// <summary>
    /// Used to mark a space where the character can be spawned. 
    /// The logic in this class pretty much just changes the color of the spawn point based on the entity for whom the spawn point is intended.
    /// But logic involving this class is pretty much only in the <see cref="CombatantSpawnManager"/> class.
    /// </summary>
    [ExecuteInEditMode]
    public class SpawnPoint: MonoBehaviour
    {
        /// <summary>
        /// Type of the spawn point, i.e. who should spawn at this point.
        /// </summary>
        public SpawnPointType Type;
        /// <summary>
        /// For each spawn point we assign a color. This way see in editor where can the enemies spawn in this room.
        /// We cannot use a dictionary, as we want to expose this to the editor.
        /// </summary>
        public List<SpawnPointColor> SpawnPointColorList;
        /// <summary>
        /// Called when the component is activated.
        /// If there is a sprite renderer for this spawn point and if there is a color for the current <see cref="Type"/>, we update the spawn point color.
        /// </summary>
        private void Awake()
        {
            var spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                return;;
            }
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
        /// <summary>
        /// Determine which <see cref="SpawnPointType"/> corresponds to the specified combatant.
        /// </summary>
        /// <param name="combatant">The combatant who wants to know where he should spawn.</param>
        /// <returns>The <see cref="SpawnPointType"/> where the <paramref name="combatant"/> should spawn.</returns>
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
        /// <summary>
        /// Retrieve the color this spawn point should have based on <see cref="SpawnPointColorList"/> and <see cref="Type"/>.
        /// </summary>
        /// <returns>The color this spawn point should have, or null if no color is assigned.</returns>
        private Color? GetColorForSpawnPoint()
        {
            return SpawnPointColorList.FirstOrDefault(spawnPointColor => spawnPointColor.Type == Type)?.TypeColor;
        }
    }
    /// <summary>
    /// Defines the color some spawn point should have.
    /// </summary>
    [Serializable]
    public class SpawnPointColor
    {
        /// <summary>
        /// The type of the spawn point for which we want the color.
        /// </summary>
        public SpawnPointType Type;
        /// <summary>
        /// The color of the spawn point.
        /// </summary>
        public Color TypeColor;
    }
    /// <summary>
    /// Specifies the kind of combatant that should appear on a spawn point.
    /// </summary>
    public enum SpawnPointType
    {
        /// <summary>
        /// The most dangerous enemies from the encounter should appear on these spawn points.
        /// </summary>
        MostPowerfulEnemy,
        /// <summary>
        /// Enemies attacking from afar should spawn at these places.
        /// </summary>
        LongRange,
        /// <summary>
        /// Enemies attacking in melee should spawn at these places.
        /// </summary>
        CloseRange,
        /// <summary>
        /// The ranger should spawn at the spawn point.
        /// </summary>
        Ranger, 
        /// <summary>
        /// The cleric should spawn at the spawn point.
        /// </summary>
        Cleric,
        /// <summary>
        /// The knight should spawn at the spawn point.
        /// </summary>
        Knight
    }
}
