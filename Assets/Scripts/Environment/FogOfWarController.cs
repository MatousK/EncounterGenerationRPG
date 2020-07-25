using System.Linq;
using Assets.Scripts.DungeonGenerator;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.Environment
{
    /// <summary>
    /// This component draws the fog of war over unexplored rooms.
    /// Draws tiles on a a sorting layer above most other objects.
    /// </summary>
    public class FogOfWarController : MonoBehaviour
    {
        /// <summary>
        /// The bounds around explored rooms which limit camera movement.
        /// </summary>
        public Bounds? ExploredAreaBounds;
        /// <summary>
        /// A tile to draw over unexplored rooms.
        /// </summary>
        public TileBase FogOfWarTile;
        /// <summary>
        /// Provides information about which rooms are explored.
        /// </summary>
        private RoomsLayout roomsLayout;
        /// <summary>
        /// The tilemap on which we should draw the fog of war overlay.
        /// </summary>
        private Tilemap tilemap;
        /// <summary>
        /// The grid on which the game is played.
        /// </summary>
        private Grid grid;

        /// <summary>
        /// Start is called before the first frame update
        /// </summary>
        void Start()
        {
            tilemap = GetComponent<Tilemap>();
            roomsLayout = FindObjectOfType<RoomsLayout>();
            grid = FindObjectOfType<Grid>();
            UpdateFogOfWar();
            foreach (var room in roomsLayout.Rooms)
            {
                room.IsExploredChanged += OnRoomExploredChanged;
            }
        }
        /// <summary>
        /// When the object is destroyed, unsubscribe from events.
        /// </summary>
        private void OnDestroy()
        {
            foreach (var room in roomsLayout.Rooms)
            {
                room.IsExploredChanged -= OnRoomExploredChanged;
            }
        }
        /// <summary>
        /// Whenever a room is explored or unesplored, update the fog of war overlay.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="exploredEventArgs">Info about which room was explored.</param>
        void OnRoomExploredChanged(object sender, RoomExploredEventArgs exploredEventArgs)
        {
            UpdateFogOfWar();
        }
        /// <summary>
        /// Draws the fog of war overlay.
        /// </summary>
        void UpdateFogOfWar()
        {
            // First, draw the black tiles over unexplored room and only after that clear the tiles from explored rooms.
            // This way corridors belonging to explored rooms won't be drawn over by the unexplored ones.
            foreach (var unexploredRoom in roomsLayout.Rooms.Where(room => !room.IsExplored))
            {
                foreach (var tilePosition in unexploredRoom.RoomSquaresPositions.Concat(unexploredRoom.ConnectedCorridorsSquares))
                {
                    tilemap.SetTile(new Vector3Int(tilePosition.x, tilePosition.y, 0), FogOfWarTile);
                }
            }

            Bounds newExploredBounds = new Bounds(Vector3.zero, Vector3.zero);
            foreach (var exploredRoom in roomsLayout.Rooms.Where(room => room.IsExplored))
            {
                foreach (var tilePosition in exploredRoom.RoomSquaresPositions.Concat(exploredRoom.ConnectedCorridorsSquares))
                {
                    var tileCellPosition = new Vector3Int(tilePosition.x, tilePosition.y, 0);
                    tilemap.SetTile(tileCellPosition, null);
                    var tileWorldPosition = grid.CellToWorld(tileCellPosition);
                    if (newExploredBounds.size == Vector3.zero)
                    {
                        newExploredBounds = new Bounds(tileWorldPosition, Vector3.one);
                    }
                    else
                    {
                        newExploredBounds.Encapsulate(tileWorldPosition);
                    }
                }
            }

            ExploredAreaBounds = newExploredBounds;
        }
    }
}
