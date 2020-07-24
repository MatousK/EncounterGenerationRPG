using Assets.ProceduralLevelGenerator.Scripts.GeneratorPipeline.Payloads.Interfaces;
using Assets.ProceduralLevelGenerator.Scripts.Pipeline;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.DungeonGenerator
{
    /// <summary>
    /// Copies game objects which were in room templates to the map.
    /// If the game object has the <see cref="RoomInfo"/> component, we add to it the information about the room in which this object is located.
    /// Uses <see cref="RestoreNonTilemapGameObjectsConfig"/> for configuration.
    /// </summary>
    /// <typeparam name="TPayload"><inheritdoc/></typeparam>
    public class RestoreNonTilemapGameObjectsTask<TPayload> : ConfigurablePipelineTask<TPayload, RestoreNonTilemapGameObjectsConfig>
        where TPayload : class, IGeneratorPayload, IGraphBasedGeneratorPayload, INamedTilemapsPayload
    {
        /// <summary>
        /// Goes through all game object in the room templates. Finds all game objects which are not tilemaps and adds them to the output map.
        /// This allows the designer to add whatever he wants to the room template, not just terrain.
        /// </summary>
        public override void Process()
        {
            var roomsData = Payload.Layout.GetAllRoomInfo();
            foreach (var room in roomsData)
            {
                foreach (Transform childTransform in room.Room.transform)
                {
                    var childObject = childTransform.gameObject;
                    if (childObject.GetComponent<Tilemap>() != null)
                    {
                        continue;
                    }
                    var childClone = Object.Instantiate(childObject, Payload.GameObject.transform);
                    var originalPositon = childClone.transform.localPosition;
                    childClone.transform.localPosition = new Vector3(originalPositon.x + room.Position.x, originalPositon.y + room.Position.y, originalPositon.z);
                    var roomInfoComponent = childClone.GetComponent<RoomInfoComponent>();
                    if (roomInfoComponent != null)
                    {
                        roomInfoComponent.RoomIndex = room.GeneratorData.Node;
                    }
                }
            }
        }
    }
}