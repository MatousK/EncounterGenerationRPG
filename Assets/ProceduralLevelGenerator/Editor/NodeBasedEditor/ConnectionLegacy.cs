using System;
using UnityEngine;

namespace Assets.ProceduralLevelGenerator.Editor.NodeBasedEditor
{
	using UnityEditor;

	public class ConnectionLegacy
	{
		public Node InPoint;
		public Node OutPoint;
		public Action<ConnectionLegacy> OnClickRemoveConnection;

		public ConnectionLegacy(Node inPoint, Node outPoint, Action<ConnectionLegacy> onClickRemoveConnection)
		{
			this.InPoint = inPoint;
			this.OutPoint = outPoint;
			this.OnClickRemoveConnection = onClickRemoveConnection;
		}

		public void Draw()
		{
			//Handles.DrawBezier(
			//	inPoint.rect.center,
			//	outPoint.rect.center,
			//	inPoint.rect.center + Vector2.left * 50f,
			//	outPoint.rect.center - Vector2.left * 50f,
			//	Color.white,
			//	null,
			//	2f
			//);

			Handles.DrawLine(InPoint.Rect.center, OutPoint.Rect.center);
#pragma warning disable CS0618 // Type or member is obsolete
			if (Handles.Button((InPoint.Rect.center + OutPoint.Rect.center) * 0.5f, Quaternion.identity, 4, 8, Handles.RectangleCap))
#pragma warning restore CS0618 // Type or member is obsolete
			{
				OnClickRemoveConnection?.Invoke(this);
			}
		}
	}
}