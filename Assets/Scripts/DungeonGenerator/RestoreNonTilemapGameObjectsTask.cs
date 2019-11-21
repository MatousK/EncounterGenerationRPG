using Assets.ProceduralLevelGenerator.Scripts.GeneratorPipeline.Payloads.Interfaces;
using Assets.ProceduralLevelGenerator.Scripts.Pipeline;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RestoreNonTilemapGameObjectsTask<TPayload> : ConfigurablePipelineTask<TPayload, RestoreNonTilemapGameObjectsConfig>
    where TPayload : class, IGeneratorPayload, IGraphBasedGeneratorPayload, INamedTilemapsPayload
{
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

            }
        }
    }
}