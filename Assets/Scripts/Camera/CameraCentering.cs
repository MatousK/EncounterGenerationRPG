using System.Linq;
using Assets.Scripts.Combat;
using Assets.Scripts.DungeonGenerator;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Camera
{
    /// <summary>
    /// This behavior centers the camera on the player characters. Will just find their center, if they're scattered, too bad.
    /// </summary>
    public class CameraCentering : MonoBehaviour
    {
        // Start is called before the first frame update
        public void Center(RoomInfo centeredRoom)
        {
            var grid = FindObjectOfType<Grid>();
            var roomBounds = centeredRoom.GetBounds(grid);
            transform.position = new Vector3(roomBounds.center.x, roomBounds.center.y, transform.position.z);
        }
    }
}
