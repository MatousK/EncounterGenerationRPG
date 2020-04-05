using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Assets.ProceduralLevelGenerator.Editor.NodeBasedEditor
{
	using UnityEditor;

	public class Node
	{
		public Rect Rect;
		public string Title;

		public bool IsDragged;
		public bool IsSelected;

		public ConnectionPoint LeftConnectionPoint;
		public ConnectionPoint RightConnectionPoint;
		public ConnectionPoint TopConnectionPoint;
		public ConnectionPoint BottomConnectionPoint;

		public GUIStyle Style;
		public GUIStyle DefaultNodeStyle;
		public GUIStyle SelectedNodeStyle;

		public Action<Node> OnRemoveNode;
		public Action<Node> OnClickNode;

		private readonly Stopwatch stopwatch = new Stopwatch();

		public Node(Vector2 position, float width, float height, GUIStyle nodeStyle, GUIStyle selectedStyle, Dictionary<ConnectionPointType, GUIStyle> connectionStyles, Action<ConnectionPoint> onClickConnectionPoint, Action<Node> onClickRemoveNode, Action<Node> onClickNode)
		{
			Rect = new Rect(position.x, position.y, width, height);
			Style = nodeStyle;
			LeftConnectionPoint = new ConnectionPoint(this, ConnectionPointType.Left, connectionStyles[ConnectionPointType.Left], onClickConnectionPoint);
			RightConnectionPoint = new ConnectionPoint(this, ConnectionPointType.Right, connectionStyles[ConnectionPointType.Right], onClickConnectionPoint);
			TopConnectionPoint = new ConnectionPoint(this, ConnectionPointType.Top, connectionStyles[ConnectionPointType.Top], onClickConnectionPoint);
			BottomConnectionPoint = new ConnectionPoint(this, ConnectionPointType.Bottom, connectionStyles[ConnectionPointType.Bottom], onClickConnectionPoint);
			DefaultNodeStyle = nodeStyle;
			SelectedNodeStyle = selectedStyle;
			OnRemoveNode = onClickRemoveNode;
			OnClickNode = onClickNode;
		}

		public void Drag(Vector2 delta)
		{
			Rect.position += delta;
		}

		public void Draw()
		{
			//LeftConnectionPoint.Draw();
			//RightConnectionPoint.Draw();
			//TopConnectionPoint.Draw();
			//BottomConnectionPoint.Draw();
			GUI.Box(Rect, Title, Style);
		}

		public bool ProcessEvents(Event e)
		{
			switch (e.type)
			{
				case EventType.MouseDown:
					if (e.button == 0)
					{
						if (Rect.Contains(e.mousePosition))
						{
							stopwatch.Restart();
							IsDragged = true;
						}
						/*else
					{
						isSelected = false;
						style = isSelected ? selectedNodeStyle : defaultNodeStyle;
						GUI.changed = true;
						OnClickNode(this);
					}*/
					}

					if (e.button == 1 && IsSelected && Rect.Contains(e.mousePosition))
					{
						ProcessContextMenu();
						e.Use();
					}
					break;

				case EventType.MouseUp:
					if (e.button == 0)
					{
						if (stopwatch.ElapsedMilliseconds < 250 && Rect.Contains(e.mousePosition))
						{
							SetSelected(!IsSelected);
							GUI.changed = true;
							OnClickNode(this);
						}
					}

					IsDragged = false;
					break;

				case EventType.MouseDrag:
					if (e.button == 0 && IsDragged)
					{
						Drag(e.delta);
						e.Use();
						return true;
					}
					break;
			}

			return false;
		}

		public void SetSelected(bool selected)
		{
			IsSelected = selected;
			Style = IsSelected ? SelectedNodeStyle : DefaultNodeStyle;
		}

		private void ProcessContextMenu()
		{
			GenericMenu genericMenu = new GenericMenu();
			genericMenu.AddItem(new GUIContent("Remove node"), false, OnClickRemoveNode);
			genericMenu.ShowAsContext();
		}

		private void OnClickRemoveNode()
		{
			OnRemoveNode?.Invoke(this);
		}
	}
}