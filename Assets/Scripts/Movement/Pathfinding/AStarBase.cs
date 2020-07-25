using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Priority_Queue;
using UnityEngine;

namespace Assets.Scripts.Movement.Pathfinding
{
    /// <summary>
    /// A class for navigation using the A* algorithm.
    /// It is a common pathfinding algorithm, google it to get an explanation of how this works.
    /// </summary>
    public abstract class AStarBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AStarBase"/>.
        /// </summary>
        /// <param name="map">The map on which the pathfinding algorithm should run.</param>
        /// <param name="startPosition">The position where the search should start.</param>
        protected AStarBase(bool[,] map, Vector2Int startPosition)
        {
            this.map = map;
            this.startPosition = startPosition;
            allNodes = new AStarNode[map.GetLength(0), map.GetLength(1)];
            closedSet = new HashSet<Vector2Int>();
            openSet = new FastPriorityQueue<AStarNode>(map.GetLength(0) * map.GetLength(1));
        }
        /// <summary>
        /// For each x,y coordinates, this array contains the A* node with pathfinding data.
        /// </summary>
        readonly AStarNode[,] allNodes;
        /// <summary>
        /// For each x,y coordinates, this array contains true if the square is passable, otherwise false.
        /// </summary>
        readonly bool[,] map;
        /// <summary>
        /// The position where the search should start.
        /// </summary>
        readonly Vector2Int startPosition;
        /// <summary>
        /// The closed set contains all positions to which we already know the shortest path.
        /// </summary>
        readonly HashSet<Vector2Int> closedSet;
        /// <summary>
        /// This contains the current frontier of the algorithm, i.e. all nodes surrounding the closed set.
        /// </summary>
        readonly FastPriorityQueue<AStarNode> openSet;
        /// <summary>
        /// We use sqrt2 often when calculating diagonal distance, so we precompute it.
        /// </summary>
        readonly float sqrt2 = (float)Math.Sqrt(2);
        AStarNode closestToTarget;
        /// <summary>
        /// Finds the shortest path based on the configuration of this class.
        /// </summary>
        /// <returns>The shortest path to the target. Or null if the path does not exist.</returns>
        public List<Vector2Int> FindPath()
        {
            AddToOpenSetIfBetter(startPosition, 0, null);
            var targetNode = RunAStarLoop();
            return ExtractPathFromTargetNode(targetNode ?? closestToTarget);
        }
        /// <summary>
        /// Called once we find the path, this returns the step by step path from the start location from the end node.
        /// </summary>
        /// <param name="targetNode">The end node of the search.</param>
        /// <returns>The path from the start position to the end position.</returns>
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
        /// <summary>
        /// Runs the A* loop. While there is something in the open set, take the best node from the open set, add it to the closed set,
        /// process the neighbours and repeat until we close the target node.
        /// </summary>
        /// <returns></returns>
        AStarNode RunAStarLoop()
        {
            while (openSet.Count != 0)
            {
                var currentNode = openSet.Dequeue();
                if (FoundTarget(currentNode.Position))
                {
                    return currentNode;
                }
                closedSet.Add(currentNode.Position);
                AddNeighboursToOpenSetIfBetter(currentNode);
            }
            return null;
        }
        /// <summary>
        /// See if we could find a better find to some position in the open space by moving in some direction from the <paramref name="node"/>.
        /// If so, they will added/updated in the open set.
        /// </summary>
        /// <param name="node"></param>
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
        /// <summary>
        /// Check the cost of the path created by moving from <paramref name="node"/> in the direction specified by the <paramref name="xModifier"/> and <paramref name="yModifier"/>.
        /// If this would result in a better path than the one we currently have to that position, this is the best path we know and should be in the open set.
        /// </summary>
        /// <param name="node">Current node of the search.</param>
        /// <param name="xModifier">How should we move on the X coordinate.</param>
        /// <param name="yModifier">How should we move on the Y coordinate.</param>
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
            // We have calculated the cost to the new space, see if it is better than what we have and update if necessary.
            AddToOpenSetIfBetter(newPosition, costToNeighbour, node);
        }
        /// <summary>
        /// Checks if the current node at the <paramref name="position"/> has worse cost than <paramref name="costToHere"/>. If so, <paramref name="previousNode"/> will replace it as
        /// the best way to get to the <paramref name="position"/>.
        /// </summary>
        /// <param name="position">Position where we want to move.</param>
        /// <param name="costToHere">How much would it cost to get there.</param>
        /// <param name="previousNode">The previous node of the search.</param>
        void AddToOpenSetIfBetter(Vector2Int position, float costToHere, AStarNode previousNode)
        {
            var oldNode = allNodes[position.x, position.y];
            // If there was a node here, we are updating, otherwise we are adding.
            if (oldNode != null)
            {
                // The node is the same, is the score better?
                // Heuristics, i.e. the distance to the target, are the same, so it is enough to just compare the cost to get here.
                if (costToHere < oldNode.CostToHere)
                {
                    oldNode.CostToHere = costToHere;
                    oldNode.PreviousNode = previousNode;
                    openSet.UpdatePriority(oldNode, costToHere + oldNode.HeuristicEstimate);
                }
            }
            else
            {
                // No old node, create a new node.
                float heuristicEstimate = CalculateHeuristic(position);
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
        /// <summary>
        /// Calculates the estimate of the cost to get the target.
        /// Must be the same or smaller than the actual cost to get there.
        /// </summary>
        /// <param name="position">The position from which we want to calculate the heuristic.</param>
        /// <returns>The estimated cost to get to the target.</returns>
        protected abstract float CalculateHeuristic(Vector2Int position);
        /// <summary>
        /// If true, the <paramref name="position"/> is the target position.
        /// </summary>
        /// <param name="position">The position which we want to evaluate.</param>
        /// <returns>True if <paramref name="position"/> is a target position, otherwise false.</returns>
        protected abstract bool FoundTarget(Vector2Int position);
    }
    /// <summary>
    /// Represents a single node in the AI search.
    /// </summary>
    class AStarNode: FastPriorityQueueNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AStarNode"/> class.
        /// </summary>
        /// <param name="costToHere">The cost to get to this position.</param>
        /// <param name="heuristicEstimate">The estimated cost to get to the target.</param>
        /// <param name="position">The position of this node.</param>
        /// <param name="previousNode">The previous node on the path, or null if this is the start node.</param>
        public AStarNode(float costToHere, float heuristicEstimate, Vector2Int position, AStarNode previousNode) 
        {
            CostToHere = costToHere;
            HeuristicEstimate = heuristicEstimate;
            Position = position;
            PreviousNode = previousNode;
            Priority = CostToHere + HeuristicEstimate;
        }
        /// <summary>
        /// The cost to get to this position.
        /// </summary>
        public float CostToHere;
        /// <summary>
        /// The estimated cost to get to the target.
        /// </summary>
        public float HeuristicEstimate;
        /// <summary>
        /// The position of this node.
        /// </summary>
        public Vector2Int Position;
        /// <summary>
        /// The previous node on the path, or null if this is the start node.
        /// </summary>
        public AStarNode PreviousNode;
    }
}