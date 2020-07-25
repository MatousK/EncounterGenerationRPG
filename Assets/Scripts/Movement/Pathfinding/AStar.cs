using UnityEngine;

namespace Assets.Scripts.Movement.Pathfinding
{
    /// <summary>
    /// <inheritdoc/>
    /// This specialization is for the classic euclidean distance and a single target position.
    /// </summary>
    public class AStar : AStarBase
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="map"><inheritdoc/></param>
        /// <param name="startPosition"><inheritdoc/></param>
        /// <param name="endPosition">The end position of the search.</param>
        public AStar(bool[,] map, Vector2Int startPosition, Vector2Int endPosition) : base(map, startPosition)
        {
            this.endPosition = endPosition;
        }
        /// <summary>
        /// The target of the pathfinding search.
        /// </summary>
        readonly Vector2Int endPosition;
        /// <summary>
        /// <inheritdoc/> Uses euclidean distance.
        /// </summary>
        /// <param name="position"><inheritdoc/></param>
        /// <returns><inheritdoc/></returns>
        protected override float CalculateHeuristic(Vector2Int position)
        {
            return Vector2Int.Distance(position, endPosition);
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="position"><inheritdoc/></param>
        /// <returns><inheritdoc/></returns>
        protected override bool FoundTarget(Vector2Int position)
        {
            return position == endPosition;
        }
    }
}