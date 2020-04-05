using System.Linq;
using Assets.Scripts.DungeonGenerator;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.Environment
{
    public class FogOfWarController : MonoBehaviour
    {
        public Bounds? ExploredAreaBounds;
        public TileBase FogOfWarTile;
        private RoomsLayout roomsLayout;
        private Tilemap tilemap;
        private Grid grid;

        // Start is called before the first frame update
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

        private void OnDestroy()
        {
            foreach (var room in roomsLayout.Rooms)
            {
                room.IsExploredChanged -= OnRoomExploredChanged;
            }
        }

        void OnRoomExploredChanged(object sender, RoomExploredEventArgs exploredEventArgs)
        {
            UpdateFogOfWar();
        }

        void UpdateFogOfWar()
        {
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
