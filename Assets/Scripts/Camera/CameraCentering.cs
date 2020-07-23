using System.Linq;
using Assets.Scripts.Combat;
using Assets.Scripts.DungeonGenerator;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Camera
{
    /// <summary>
    /// This component can center the camera on some specific room without animations.
    /// </summary>
    public class CameraCentering : MonoBehaviour
    {
        /// <summary>
        /// Immediately change the camera so the specified room is in the center of the view.
        /// </summary>
        /// <param name="centeredRoom">The room that should be in the center of the view.</param>
        public void Center(RoomInfo centeredRoom)
        {
            var grid = FindObjectOfType<Grid>();
            var roomBounds = centeredRoom.GetBounds(grid);
            transform.position = new Vector3(roomBounds.center.x, roomBounds.center.y, transform.position.z);
        }
    }
}
