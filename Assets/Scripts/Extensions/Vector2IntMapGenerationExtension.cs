using GeneralAlgorithms.DataStructures.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class Vector2IntMapGenerationExtension
 {
    public static IntVector2 ToMapGeneratorVector(this Vector2Int Vector)
    {
        return new IntVector2(Vector.x, Vector.y);
    }
    public static Vector2Int ToUnityVector(this IntVector2 Vector)
    {
        return new Vector2Int(Vector.X, Vector.Y);
    }
}
