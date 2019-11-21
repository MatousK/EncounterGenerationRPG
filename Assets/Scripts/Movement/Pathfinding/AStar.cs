using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Priority_Queue;

public class AStar : AStarBase
{
    public AStar(bool[,] map, Vector2Int startPosition, Vector2Int endPosition) : base(map, startPosition)
    {
        this.endPosition = endPosition;
    }
    Vector2Int endPosition;

    protected override float CalculateHeuristic(Vector2Int position)
    {
        return Vector2Int.Distance(position, endPosition);
    }

    protected override bool FoundTarget(Vector2Int position)
    {
        return position == endPosition;
    }
}