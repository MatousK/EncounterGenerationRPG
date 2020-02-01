using Assets.ProceduralLevelGenerator.Scripts.GeneratorPipeline.RoomTemplates.TilemapLayers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Basic implementation of tilemap layers handler.
/// </summary>
[CreateAssetMenu(menuName = "Dungeon generator/Encounter Generation RPG/Tilemap Layers Handler", fileName = "EGTilemapLayersHandler")]
public class EncounterGeneratorTilemapLayersHandler : TilemapLayersHandler
{
    public string WallsSortingLayer;
    public int WallsSortingOrder;
    public string FloorSortingLayer;
    public int FloorSortingOrder;
    public string CollidableSortingLayer;
    public int CollidableSortingOrder;
    public string Other1SortingLayer;
    public int Other1SortingOrder;
    public string Other2SortingLayer;
    public int Other2SortingOrder;
    public string Other3SortingLayer;
    public int Other3SortingOrder;

    /// <summary>
    /// Initializes individual tilemap layers.
    /// </summary>
    /// <param name="gameObject"></param>
    public override void InitializeTilemaps(GameObject gameObject)
    {
        var wallsTilemapObject = CreateTilemapGameObject("Walls", gameObject, WallsSortingLayer, WallsSortingOrder);
        AddCompositeCollider(wallsTilemapObject);

        CreateTilemapGameObject("Floor", gameObject, FloorSortingLayer, FloorSortingOrder);

        var collideableTilemapObject = CreateTilemapGameObject("Collideable", gameObject, CollidableSortingLayer, CollidableSortingOrder);
        AddCompositeCollider(collideableTilemapObject);

        CreateTilemapGameObject("Other 1", gameObject, Other1SortingLayer, Other1SortingOrder);

        CreateTilemapGameObject("Other 2", gameObject, Other2SortingLayer, Other2SortingOrder);

        CreateTilemapGameObject("Other 3", gameObject, Other3SortingLayer, Other3SortingOrder);
    }

    protected GameObject CreateTilemapGameObject(string name, GameObject parentObject, string sortingLayerName, int sortingOrder)
    {
        var tilemapObject = new GameObject(name);
        tilemapObject.transform.SetParent(parentObject.transform);
        tilemapObject.AddComponent<Tilemap>();
        var tilemapRenderer = tilemapObject.AddComponent<TilemapRenderer>();
        tilemapRenderer.sortingOrder = sortingOrder;
        tilemapRenderer.sortingLayerName = sortingLayerName;

        return tilemapObject;
    }
}
