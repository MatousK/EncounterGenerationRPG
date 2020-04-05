﻿using System.Collections.Generic;
using Assets.Scripts.EncounterGenerator.Model;
using UnityEngine;

namespace Assets.Scripts.EncounterGenerator.Configuration
{
    /// <summary>
    /// Parameters used for generating a group of monsters.
    /// </summary>
    public struct GenerateMonsterGroupParameters
    {
        /// <summary>
        /// How monsters with which roles should be generated.
        /// </summary>
        public EncounterDefinition RequestedMonsters;
        /// <summary>
        /// Which monsters should be more likely to appear. 0 is neutral, positive is they should appear more likely, negative is that they should not appear if possible.
        /// </summary>
        public Dictionary<GameObject, float> MonsterPriorities;
    }
}