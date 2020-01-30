using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FogOfWarController : MonoBehaviour
{
    public TileBase FogOfWarTile;
    RoomsLayout roomsLayout;
    Tilemap tilemap;

    // Start is called before the first frame update
    void Start()
    {
        tilemap = GetComponent<Tilemap>();
        roomsLayout = FindObjectOfType<RoomsLayout>();
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
            foreach (var tilePosition in unexploredRoom.RoomSquaresPositions)
            {
                tilemap.SetTile(new Vector3Int(tilePosition.x, tilePosition.y, 0), FogOfWarTile);
            }
        }
        foreach (var exploredRoom in roomsLayout.Rooms.Where(room => room.IsExplored))
        {
            foreach (var tilePosition in exploredRoom.RoomSquaresPositions)
            {
                tilemap.SetTile(new Vector3Int(tilePosition.x, tilePosition.y, 0), null);
            }
        }
    }
}
