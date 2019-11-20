using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Priority_Queue;

class AStar
{
    public AStar(bool[,] map, Vector2Int startPosition, Vector2Int endPosition)
    {
        this.map = map;
        this.startPosition = startPosition;
        this.endPosition = endPosition;
        allNodes = new AStarNode[map.GetLength(0), map.GetLength(1)];
        closedSet = new HashSet<Vector2Int>();
        openSet = new FastPriorityQueue<AStarNode>(map.GetLength(0) * map.GetLength(1));
    }
    AStarNode[,] allNodes;
    bool[,] map;
    Vector2Int startPosition;
    Vector2Int endPosition;
    HashSet<Vector2Int> closedSet;
    FastPriorityQueue<AStarNode> openSet;
    readonly float sqrt2 = (float)(Math.Sqrt(2));
    AStarNode closestToTarget;

    public List<Vector2Int> FindPath()
    {
        AddToOpenSetIfBetter(startPosition, 0, null);
        var targetNode = RunAStarLoop();
        return ExtractPathFromTargetNode(targetNode ?? closestToTarget);
    }

    List<Vector2Int> ExtractPathFromTargetNode(AStarNode targetNode)
    {
        List<Vector2Int> toReturn = new List<Vector2Int>();
        var currentNode = targetNode;
        while (currentNode != null)
        {
            toReturn.Add(currentNode.Position);
            currentNode = currentNode.PreviousNode;
        }
        toReturn.Reverse();
        return toReturn;
    }

    AStarNode RunAStarLoop()
    {
        while (openSet.Count != 0)
        {
            var currentNode = openSet.Dequeue();
            if (currentNode.Position == endPosition)
            {
                return currentNode;
            }
            closedSet.Add(currentNode.Position);
            AddNeighboursToOpenSetIfBetter(currentNode);
        }
        return null;
    }

    void AddNeighboursToOpenSetIfBetter(AStarNode node)
    {
        TryAddNeighbourToOpenSetIfBetter(node, -1, 0);
        TryAddNeighbourToOpenSetIfBetter(node, 1, 0);
        TryAddNeighbourToOpenSetIfBetter(node, -1, -1);
        TryAddNeighbourToOpenSetIfBetter(node, 0, -1);
        TryAddNeighbourToOpenSetIfBetter(node, 1, -1);
        TryAddNeighbourToOpenSetIfBetter(node, -1, 1);
        TryAddNeighbourToOpenSetIfBetter(node, 0, 1);
        TryAddNeighbourToOpenSetIfBetter(node, 1, 1);
    }

    void TryAddNeighbourToOpenSetIfBetter(AStarNode node, int xModifier, int yModifier)
    {
        Vector2Int newPosition = new Vector2Int(node.Position.x + xModifier, node.Position.y + yModifier);
        // Check if the target place is passable, if it is within bounds and if it is not yet in the closed set.
        if (closedSet.Contains(newPosition) ||
            newPosition.x < 0 ||
            newPosition.y < 0 || 
            newPosition.x >= map.GetLength(0) ||
            newPosition.y >= map.GetLength(1) || 
            !map[newPosition.x, newPosition.y])
        {
            return;
        }
        bool isDiagonal = xModifier != 0 && yModifier != 0;
        if (isDiagonal)
        {
            // Diagonal movement is impossible if the squares between them are not also free - for example, north-west is only possible if both north and west are free.
            if (!map[newPosition.x - xModifier, newPosition.y] || !map[newPosition.x, newPosition.y - yModifier])
            {
                return;
            }
        }
        var costToNeighbour = node.CostToHere + (isDiagonal ? sqrt2 : 1);
        AddToOpenSetIfBetter(newPosition, costToNeighbour, node);
    }

    void AddToOpenSetIfBetter(Vector2Int position, float costToHere, AStarNode previousNode)
    {
        var oldNode = allNodes[position.x, position.y];
        // If there was a node here, we are updating, otherwise we are adding.
        if (oldNode != null)
        {
            // Heuristics are the same, enough to just compare the cost to get here.
            if (costToHere < oldNode.CostToHere)
            {
                oldNode.CostToHere = costToHere;
                oldNode.PreviousNode = previousNode;
                openSet.UpdatePriority(oldNode, costToHere + oldNode.HeuristicEstimate);
            }
        }
        else
        {
            float heuristicEstimate = Vector2Int.Distance(position, endPosition);
            allNodes[position.x, position.y] = new AStarNode(costToHere, heuristicEstimate, position, previousNode);
            openSet.Enqueue(allNodes[position.x, position.y], allNodes[position.x, position.y].Priority);
            // Distance to target does not change, so finding the closest position to target should be possible here.
            float closestToTargetDistance = closestToTarget?.HeuristicEstimate ?? float.MaxValue;
            if (closestToTargetDistance > heuristicEstimate)
            {
                closestToTarget = allNodes[position.x, position.y];
            }
        }
    }
}

class AStarNode: FastPriorityQueueNode
{
    public AStarNode(float costToHere, float heuristicEstimate, Vector2Int position, AStarNode previousNode) 
    {
        CostToHere = costToHere;
        HeuristicEstimate = heuristicEstimate;
        Position = position;
        PreviousNode = previousNode;
        Priority = CostToHere + HeuristicEstimate;
    }
    public float CostToHere;
    public float HeuristicEstimate;
    public Vector2Int Position;
    public AStarNode PreviousNode;
}