using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class AStar
{
    AStar(bool[,] map, Vector2Int start, Vector2Int end)
    {
        this.map = map;
        this.start = start;
        this.end = end;
    }

    bool[,] map;
    Vector2Int start;
    Vector2Int end;
    Dictionary<Vector2Int, AStarNode> closedSet;
    //FastPriorityQueue


    List<Vector2Int> FindPath()
    {
        return null;
    }
}

class AStarNode
{
    public Vector2Int Position;
    public AStarNode PreviousNode;
    public float Cost;
}