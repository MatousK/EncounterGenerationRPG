using System.Collections.Generic;
using Assets.ProceduralLevelGenerator.Scripts.Data.Graphs;
using Assets.Scripts.EncounterGenerator.Configuration;
using UnityEngine;

namespace Assets.Scripts.DungeonGenerator
{
    /// <summary>
    /// Extension of the Room class from Dungeon generator that includes info about encounter difficulties and treasures.
    /// </summary>
    public class RoomWithEncounter: Room
    {
        /// <summary>
        /// The encounter that should appear here.
        /// </summary>
        public EncounterConfiguration EncounterConfiguration;
        /// <summary>
        /// How many chests with healing potions should spawn here.
        /// </summary>
        public int HealingPotionChests;
        /// <summary>
        /// How many chests with health bonus should spawn here.
        /// </summary>
        public int HealthBonusChests;
        /// <summary>
        /// How many chests with damage bonus should spawn here.
        /// </summary>
        public int DamageBonusChests;
        /// <summary>
        /// If matrix based generator is disabled, these monsters should spawn here.
        /// </summary>
        public List<GameObject> StaticMonsters;
    }
}
