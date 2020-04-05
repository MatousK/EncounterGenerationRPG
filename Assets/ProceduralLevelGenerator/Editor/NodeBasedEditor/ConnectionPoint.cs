namespace Assets.ProceduralLevelGenerator.Editor.NodeBasedEditor
{
	using System;
	using UnityEngine;

	public enum ConnectionPointType { Left, Top, Right, Bottom }

	public class ConnectionPoint
	{
		public Rect Rect;

		public ConnectionPointType Type;

		public Node Node;

		public GUIStyle Style;

		public Action<ConnectionPoint> OnClickConnectionPoint;

		public ConnectionPoint(Node node, ConnectionPointType type, GUIStyle style, Action<ConnectionPoint> onClickConnectionPoint)
		{
			this.Node = node;
			this.Type = type;
			this.Style = style;
			this.OnClickConnectionPoint = onClickConnectionPoint;
			Rect = new Rect(0, 0, 10f, 20f);
		}

		public void Draw()
		{
		

			switch (Type)
			{
				case ConnectionPointType.Left:
					Rect.x = Node.Rect.x - Rect.width + 8f;
					Rect.y = Node.Rect.y + (Node.Rect.height * 0.5f) - Rect.height * 0.5f;
					break;

				case ConnectionPointType.Right:
					Rect.x = Node.Rect.x + Node.Rect.width - 8f;
					Rect.y = Node.Rect.y + (Node.Rect.height * 0.5f) - Rect.height * 0.5f;
					break;

				case ConnectionPointType.Top:
					Rect.y = Node.Rect.y + Rect.height + 8f;
					Rect.x = Node.Rect.x + (Node.Rect.width * 0.5f) - Rect.width * 0.5f;
					break;

				case ConnectionPointType.Bottom:
					Rect.y = Node.Rect.y - Rect.height + 8f;
					Rect.x = Node.Rect.x + (Node.Rect.width * 0.5f) - Rect.width * 0.5f;
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}

            if (GUI.Button(Rect, "", Style))
			{ 
                OnClickConnectionPoint?.Invoke(this);
			}
		}
	}
}