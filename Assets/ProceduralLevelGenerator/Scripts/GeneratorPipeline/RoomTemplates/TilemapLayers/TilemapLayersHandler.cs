namespace Assets.ProceduralLevelGenerator.Scripts.GeneratorPipeline.RoomTemplates.TilemapLayers
{
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Tilemaps;

	/// <summary>
	/// Basic implementation of tilemap layers handler.
	/// </summary>
	[CreateAssetMenu(menuName = "Dungeon generator/Pipeline/Tilemap layers handler", fileName = "TilemapLayersHandler")]
	public class TilemapLayersHandler : AbstractTilemapLayersHandler
	{
		/// <summary>
		/// Initializes individual tilemap layers.
		/// </summary>
		/// <param name="gameObject"></param>
		public override void InitializeTilemaps(GameObject gameObject)
		{
			var wallsTilemapObject = CreateTilemapGameObject("Walls", gameObject, 0);
			AddCompositeCollider(wallsTilemapObject);

			CreateTilemapGameObject("Floor", gameObject, 1);

			var collideableTilemapObject = CreateTilemapGameObject("Collideable", gameObject, 2);
			AddCompositeCollider(collideableTilemapObject);

			CreateTilemapGameObject("Other 1", gameObject, 3);

			CreateTilemapGameObject("Other 2", gameObject, 4);

			CreateTilemapGameObject("Other 3", gameObject, 5);
		}

		protected GameObject CreateTilemapGameObject(string tilemapName, GameObject parentObject, int sortingOrder)
		{
			var tilemapObject = new GameObject(tilemapName);
			tilemapObject.transform.SetParent(parentObject.transform);
			tilemapObject.AddComponent<Tilemap>();
			var tilemapRenderer = tilemapObject.AddComponent<TilemapRenderer>();
			tilemapRenderer.sortingOrder = sortingOrder;

			return tilemapObject;
		}

		protected void AddCompositeCollider(GameObject gameObject)
		{
			var tilemapCollider2D = gameObject.AddComponent<TilemapCollider2D>();
			tilemapCollider2D.usedByComposite = true;

			gameObject.AddComponent<CompositeCollider2D>();
			gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
		}
	}
}