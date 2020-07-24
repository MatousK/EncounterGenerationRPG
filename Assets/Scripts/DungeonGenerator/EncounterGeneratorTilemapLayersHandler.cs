using Assets.ProceduralLevelGenerator.Scripts.GeneratorPipeline.RoomTemplates.TilemapLayers;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.DungeonGenerator
{
    /// <summary>
    /// Adds all tile maps used by the dungeon generator to the game.
    /// This class extends the basic functionality to allow the designer to specify sorting layers and sorting orders for the individual tilemaps.
    /// </summary>
    [CreateAssetMenu(menuName = "Dungeon generator/Encounter Generation RPG/Tilemap Layers Handler", fileName = "EGTilemapLayersHandler")]
    public class EncounterGeneratorTilemapLayersHandler : TilemapLayersHandler
    {
        /// <summary>
        /// The sorting layer for the walls tile map.
        /// </summary>
        public string WallsSortingLayer;
        /// <summary>
        /// The sorting order for the walls tile map.
        /// </summary>
        public int WallsSortingOrder;
        /// <summary>
        /// The sorting layer for the floor tile map.
        /// </summary>
        public string FloorSortingLayer;
        /// <summary>
        /// The sorting order for the floor tile map.
        /// </summary>
        public int FloorSortingOrder;
        /// <summary>
        /// The sorting layer for the collidable tile map.
        /// </summary>
        public string CollidableSortingLayer;
        /// <summary>
        /// The sorting order for the collidable tile map.
        /// </summary>
        public int CollidableSortingOrder;
        /// <summary>
        /// The sorting layer for the Other 1 tile map.
        /// </summary>
        public string Other1SortingLayer;
        /// <summary>
        /// The sorting order for the Other 1 tile map.
        /// </summary>
        public int Other1SortingOrder;
        /// <summary>
        /// The sorting layer for the Other 2 tile map.
        /// </summary>
        public string Other2SortingLayer;
        /// <summary>
        /// The sorting order for the Other 2 tile map.
        /// </summary>
        public int Other2SortingOrder;
        /// <summary>
        /// The sorting layer for the Other 3 tile map.
        /// </summary>
        public string Other3SortingLayer;
        /// <summary>
        /// The sorting order for the Other 3 tile map.
        /// </summary>
        public int Other3SortingOrder;

        /// <summary>
        /// Initializes individual tilemap layers and assigns to them the correct sorting order.
        /// </summary>
        /// <param name="gameObject">The game object to which the tile maps should be added.</param>
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
        /// <summary>
        /// Initializes the specified tilemap and adds it to the parent game object.
        /// </summary>
        /// <param name="name">Name of the tilemap layer, visible in the game object tree.</param>
        /// <param name="parentObject">The object to which the tile maps should be added.</param>
        /// <param name="sortingLayerName">Name of the sorting layer that should be assigned to the tile map.</param>
        /// <param name="sortingOrder">Sorting order within the layer that should be assigned to the tile map.</param>
        /// <returns></returns>
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
}
