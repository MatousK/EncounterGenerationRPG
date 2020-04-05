using UnityEngine;

namespace Assets.Scripts.Movement.Pathfinding
{
    public class AStar : AStarBase
    {
        public AStar(bool[,] map, Vector2Int startPosition, Vector2Int endPosition) : base(map, startPosition)
        {
            this.endPosition = endPosition;
        }

        readonly Vector2Int endPosition;

        protected override float CalculateHeuristic(Vector2Int position)
        {
            return Vector2Int.Distance(position, endPosition);
        }

        protected override bool FoundTarget(Vector2Int position)
        {
            return position == endPosition;
        }
    }
}