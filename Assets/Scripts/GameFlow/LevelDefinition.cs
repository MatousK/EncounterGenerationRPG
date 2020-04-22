using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.ProceduralLevelGenerator.Scripts.Data.Graphs;
using UnityEngine;

namespace Assets.Scripts.GameFlow
{
    [CreateAssetMenu(menuName = "Encounter generator/Level Definition", fileName = "LevelDefinition")]
    public class LevelDefinition: ScriptableObject
    {
        public LevelGraph[] PossibleLevelGraphs;

    }
}
